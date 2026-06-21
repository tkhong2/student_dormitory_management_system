using Microsoft.AspNetCore.Mvc;
using BillingMaintenanceService.Application.Interfaces;
using System.Text.Json;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SepayController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly ILogger<SepayController> _logger;
        private readonly IConfiguration _configuration;

        public SepayController(
            IInvoiceRepository invoiceRepository,
            IPaymentRepository paymentRepository,
            ILogger<SepayController> logger,
            IConfiguration configuration)
        {
            _invoiceRepository = invoiceRepository;
            _paymentRepository = paymentRepository;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// DEBUG: Log raw request body to see exact format from SePay
        /// </summary>
        [HttpPost("webhook-debug")]
        public async Task<IActionResult> WebhookDebug()
        {
            try
            {
                using var reader = new StreamReader(Request.Body);
                var rawBody = await reader.ReadToEndAsync();
                
                _logger.LogInformation("=== RAW SEPAY REQUEST ===");
                _logger.LogInformation("Headers: {Headers}", JsonSerializer.Serialize(Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())));
                _logger.LogInformation("Body: {Body}", rawBody);
                _logger.LogInformation("Content-Type: {ContentType}", Request.ContentType);
                _logger.LogInformation("=========================");
                
                return Ok(new { 
                    message = "Debug info logged",
                    receivedBody = rawBody,
                    contentType = Request.ContentType,
                    headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString())
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in webhook debug");
                return StatusCode(500, new { message = "Error", error = ex.Message });
            }
        }

        /// <summary>
        /// Webhook endpoint to receive payment notifications from Sepay
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] SepayWebhookDto? webhookData)
        {
            try
            {
                // Log raw body first for debugging
                Request.EnableBuffering();
                using var reader = new StreamReader(Request.Body, leaveOpen: true);
                var rawBody = await reader.ReadToEndAsync();
                Request.Body.Position = 0;
                
                _logger.LogInformation("=== SEPAY WEBHOOK RECEIVED ===");
                _logger.LogInformation("Raw Body: {RawBody}", rawBody);
                _logger.LogInformation("Content-Type: {ContentType}", Request.ContentType);
                
                // Check if model binding failed
                if (webhookData == null)
                {
                    _logger.LogError("Model binding failed - webhookData is null. Raw body: {Body}", rawBody);
                    return BadRequest(new { message = "Invalid request format", receivedBody = rawBody });
                }
                
                _logger.LogInformation("Parsed Sepay webhook: {Data}", JsonSerializer.Serialize(webhookData));

                // Extract content field (nội dung chuyển khoản)
                string transferContent = webhookData.content ?? "";
                decimal amount = webhookData.transferAmount;
                
                _logger.LogInformation("Transfer content: {Content}, Amount: {Amount}", transferContent, amount);

                // Check if this is a utility payment (format: TIENICH {usageLogId} {studentCode})
                var utilityMatch = System.Text.RegularExpressions.Regex.Match(transferContent, @"TIENICH\s+(\d+)");
                if (utilityMatch.Success)
                {
                    int usageLogId = int.Parse(utilityMatch.Groups[1].Value);
                    _logger.LogInformation("Detected utility payment for usageLogId: {Id}", usageLogId);

                    // Call RoomBuildingService to mark as paid
                    try
                    {
                        using var httpClient = new HttpClient();
                        var roomBuildingServiceUrl = _configuration["Services:RoomBuildingService"] ?? "http://localhost:5003";
                        var apiUrl = $"{roomBuildingServiceUrl}/api/utilityusagelogs/{usageLogId}/mark-paid-from-webhook";
                        
                        var markPaidDto = new
                        {
                            transactionCode = webhookData.id.ToString(),
                            amount = amount
                        };
                        
                        var jsonContent = new StringContent(
                            System.Text.Json.JsonSerializer.Serialize(markPaidDto),
                            System.Text.Encoding.UTF8,
                            "application/json"
                        );
                        
                        var response = await httpClient.PostAsync(apiUrl, jsonContent);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Successfully marked utility usage {Id} as paid", usageLogId);
                            return Ok(new { 
                                message = "Utility payment processed successfully",
                                usageLogId = usageLogId,
                                isPaid = true
                            });
                        }
                        else
                        {
                            _logger.LogWarning("Failed to mark utility usage as paid: {StatusCode}", response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error calling RoomBuildingService for utility payment");
                    }
                    
                    return Ok(new { message = "Utility payment webhook received but failed to update" });
                }

                // Extract invoice code from content (format: "PTT202606001 SV000133" or just "PTT202606001")
                var invoiceCode = ExtractInvoiceCode(transferContent);
                
                if (string.IsNullOrEmpty(invoiceCode))
                {
                    _logger.LogWarning("Could not extract invoice code from transfer content: {Content}", transferContent);
                    return Ok(new { message = "No invoice code found in transfer content" });
                }

                _logger.LogInformation("Extracted invoice code: {Code}", invoiceCode);

                // Find invoice by code
                var invoices = await _invoiceRepository.GetAllAsync();
                var invoice = invoices.FirstOrDefault(i => i.InvoiceCode == invoiceCode);

                if (invoice == null)
                {
                    _logger.LogWarning("Invoice not found: {Code}", invoiceCode);
                    return Ok(new { message = "Invoice not found" });
                }

                // Check if already paid
                if (invoice.Status == "Paid")
                {
                    _logger.LogInformation("Invoice already paid: {Code}", invoiceCode);
                    return Ok(new { message = "Invoice already paid" });
                }

                // Get payment amount
                var amountReceived = amount;
                
                _logger.LogInformation("Processing payment: Invoice={Code}, Amount={Amount}", invoiceCode, amountReceived);

                // Get system user or first admin user for ReceivedByUserId
                var systemUser = (await _invoiceRepository.GetAllAsync())
                    .Select(i => i.CreatedByUser)
                    .FirstOrDefault(u => u != null && u.Role == "Admin");
                
                int receivedByUserId = systemUser?.Id ?? invoice.CreatedByUserId;
                string receivedByName = systemUser?.FullName ?? "System (Sepay Auto)";

                // Create payment record
                var payment = new Domain.Entities.Payment
                {
                    InvoiceId = invoice.Id,
                    Amount = amountReceived,
                    Method = "BankTransfer",
                    PaymentDate = DateOnly.FromDateTime(DateTime.Now),
                    PaidAt = DateTime.Now,
                    TransactionCode = webhookData.id.ToString(),
                    BankName = webhookData.gateway ?? "BIDV",
                    BankAccountNumber = webhookData.accountNumber,
                    ReceivedByUserId = receivedByUserId,
                    ReceivedByName = receivedByName,
                    Note = $"Thanh toán qua Sepay - {transferContent}"
                };

                await _paymentRepository.AddAsync(payment);

                // Update invoice
                invoice.PaidAmount += amountReceived;
                invoice.DebtAmount = invoice.TotalAmount - invoice.PaidAmount;
                
                if (invoice.DebtAmount <= 0)
                {
                    invoice.Status = "Paid";
                }
                else if (invoice.PaidAmount > 0)
                {
                    invoice.Status = "PartialPaid";
                }

                await _invoiceRepository.UpdateAsync(invoice);

                _logger.LogInformation("Payment processed successfully: Invoice={Code}, NewStatus={Status}", 
                    invoiceCode, invoice.Status);

                return Ok(new { 
                    message = "Payment processed successfully",
                    invoiceCode = invoiceCode,
                    status = invoice.Status,
                    paidAmount = invoice.PaidAmount,
                    debtAmount = invoice.DebtAmount
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Sepay webhook");
                return StatusCode(500, new { message = "Error processing payment", error = ex.Message });
            }
        }

        /// <summary>
        /// Manually check transactions from Sepay for a specific invoice
        /// </summary>
        [HttpPost("check-payment")]
        public async Task<IActionResult> CheckPayment([FromBody] CheckPaymentDto dto)
        {
            try
            {
                _logger.LogInformation("Manual payment check for invoice: {Code}", dto.InvoiceCode);

                // Find invoice
                var invoices = await _invoiceRepository.GetAllAsync();
                var invoice = invoices.FirstOrDefault(i => i.InvoiceCode == dto.InvoiceCode);

                if (invoice == null)
                {
                    return NotFound(new { message = "Invoice not found" });
                }

                // Get Sepay configuration
                var sepayAccountNumber = _configuration["Sepay:AccountNumber"] ?? "8871422018";
                var sepayApiKey = _configuration["Sepay:ApiKey"];
                
                // If API key exists, call Sepay API to check real transactions
                if (!string.IsNullOrEmpty(sepayApiKey))
                {
                    try
                    {
                        using var httpClient = new HttpClient();
                        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {sepayApiKey}");
                        
                        // Get transactions from Sepay (last 30 days)
                        var fromDate = DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd");
                        var toDate = DateTime.Now.ToString("yyyy-MM-dd");
                        var sepayUrl = $"https://my.sepay.vn/userapi/transactions/list?account_number={sepayAccountNumber}&from_date={fromDate}&to_date={toDate}";
                        
                        _logger.LogInformation("Calling Sepay API: {Url}", sepayUrl);
                        
                        var response = await httpClient.GetAsync(sepayUrl);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            var transactions = System.Text.Json.JsonSerializer.Deserialize<SepayTransactionsResponse>(content);
                            
                            // Find transaction matching this invoice
                            var matchingTransaction = transactions?.Transactions?.FirstOrDefault(t => 
                                t.TransactionContent?.Contains(invoice.InvoiceCode) == true &&
                                t.Amount >= invoice.DebtAmount
                            );
                            
                            if (matchingTransaction != null && invoice.Status != "Paid")
                            {
                                _logger.LogInformation("Found matching transaction on Sepay: {TransactionId}", matchingTransaction.Id);
                                
                                // Process the payment automatically
                                var webhookData = new SepayWebhookDto
                                {
                                    id = int.TryParse(matchingTransaction.Id, out int txId) ? txId : 0,
                                    gateway = matchingTransaction.BankBrandName,
                                    transactionDate = matchingTransaction.TransactionDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                    accountNumber = sepayAccountNumber,
                                    subAccount = null,
                                    transferType = "In",
                                    transferAmount = matchingTransaction.Amount,
                                    accumulated = matchingTransaction.Amount,
                                    code = matchingTransaction.BankBrandName,
                                    content = matchingTransaction.TransactionContent,
                                    referenceCode = matchingTransaction.Id,
                                    description = "Auto-detected from SePay API"
                                };
                                
                                await Webhook(webhookData);
                                
                                // Refresh invoice status
                                invoice = await _invoiceRepository.GetByIdAsync(invoice.Id);
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Sepay API returned error: {StatusCode}", response.StatusCode);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error calling Sepay API");
                        // Continue to return current invoice status even if Sepay API fails
                    }
                }
                
                // Return current invoice status
                return Ok(new
                {
                    invoiceCode = invoice.InvoiceCode,
                    status = invoice.Status,
                    totalAmount = invoice.TotalAmount,
                    paidAmount = invoice.PaidAmount,
                    debtAmount = invoice.DebtAmount,
                    message = invoice.Status == "Paid" ? "Đã thanh toán đầy đủ" : "Chưa thanh toán đầy đủ",
                    checkedSepayApi = !string.IsNullOrEmpty(sepayApiKey)
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking payment");
                return StatusCode(500, new { message = "Error checking payment", error = ex.Message });
            }
        }

        /// <summary>
        /// TEST ONLY: Simulate a successful payment (for testing without real bank transfer)
        /// </summary>
        [HttpPost("simulate-payment")]
        public async Task<IActionResult> SimulatePayment([FromBody] SimulatePaymentDto dto)
        {
            try
            {
                _logger.LogInformation("TEST: Simulating payment for invoice: {Code}", dto.InvoiceCode);

                // Find invoice
                var invoices = await _invoiceRepository.GetAllAsync();
                var invoice = invoices.FirstOrDefault(i => i.InvoiceCode == dto.InvoiceCode);

                if (invoice == null)
                {
                    return NotFound(new { message = "Invoice not found" });
                }

                // Check if already paid
                if (invoice.Status == "Paid")
                {
                    return Ok(new { message = "Invoice already paid" });
                }

                // Simulate webhook data with new DTO format
                var webhookData = new SepayWebhookDto
                {
                    id = 0,
                    gateway = "SePay",
                    transactionDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    accountNumber = "8871422018",
                    subAccount = null,
                    transferType = "In",
                    transferAmount = dto.Amount > 0 ? dto.Amount : invoice.DebtAmount,
                    accumulated = dto.Amount > 0 ? dto.Amount : invoice.DebtAmount,
                    code = "SEPAYTLS",
                    content = $"{invoice.InvoiceCode} {invoice.StudentCode}",
                    referenceCode = $"TEST{DateTime.Now:yyyyMMddHHmmss}",
                    description = "Test payment simulation"
                };

                // Call webhook method
                var result = await Webhook(webhookData);
                
                _logger.LogInformation("TEST: Payment simulation completed for invoice: {Code}", dto.InvoiceCode);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error simulating payment");
                return StatusCode(500, new { message = "Error simulating payment", error = ex.Message });
            }
        }

        private string ExtractInvoiceCode(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return "";

            // Look for pattern PT[DTRO] followed by numbers (e.g., PTT202606001, PTD202606002, PTR202606003, PTO202606004)
            var match = System.Text.RegularExpressions.Regex.Match(content, @"PT[DTRO]\d+");
            return match.Success ? match.Value : "";
        }

        /// <summary>
        /// Check payment for utility usage (from RoomBuildingService)
        /// Format: TIENICH {usageLogId} {studentCode}
        /// </summary>
        [HttpPost("check-utility-payment")]
        public async Task<IActionResult> CheckUtilityPayment([FromBody] CheckUtilityPaymentDto dto)
        {
            try
            {
                _logger.LogInformation("Checking utility payment for usageLogId: {Id}", dto.UsageLogId);

                // Call RoomBuildingService to check and update payment status
                using var httpClient = new HttpClient();
                
                // Get RoomBuildingService URL from configuration or use default
                var roomBuildingServiceUrl = _configuration["Services:RoomBuildingService"] ?? "http://localhost:5002";
                var apiUrl = $"{roomBuildingServiceUrl}/api/utilityusagelogs/{dto.UsageLogId}/check-payment";
                
                _logger.LogInformation("Calling RoomBuildingService: {Url}", apiUrl);
                
                var response = await httpClient.PostAsync(apiUrl, null);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<CheckUtilityPaymentResponseDto>(content);
                    
                    return Ok(result);
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error from RoomBuildingService: {StatusCode}, {Content}", response.StatusCode, errorContent);
                    return StatusCode((int)response.StatusCode, new { message = "Error checking utility payment", error = errorContent });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking utility payment");
                return StatusCode(500, new { message = "Error checking utility payment", error = ex.Message });
            }
        }
    }

    // DTOs for Sepay webhook - Simple version matching SePay's actual format
    public class SepayWebhookDto
    {
        public int id { get; set; }
        public string? gateway { get; set; }
        public string? transactionDate { get; set; }
        public string? accountNumber { get; set; }
        public string? subAccount { get; set; }
        public string? transferType { get; set; }
        public decimal transferAmount { get; set; }
        public decimal accumulated { get; set; }
        public string? code { get; set; }
        public string? content { get; set; }
        public string? referenceCode { get; set; }
        public string? description { get; set; }
    }

    public class CheckPaymentDto
    {
        public string InvoiceCode { get; set; } = null!;
    }

    public class CheckUtilityPaymentDto
    {
        public int UsageLogId { get; set; }
    }

    public class CheckUtilityPaymentResponseDto
    {
        public int Id { get; set; }
        public bool IsPaid { get; set; }
        public decimal? FeeCharged { get; set; }
        public string? Message { get; set; }
    }

    public class SimulatePaymentDto
    {
        public string InvoiceCode { get; set; } = null!;
        public decimal Amount { get; set; } = 0; // 0 = pay full debt
    }

    // Sepay API Response DTOs
    public class SepayTransactionsResponse
    {
        public bool Status { get; set; }
        public List<SepayTransaction>? Transactions { get; set; }
    }

    public class SepayTransaction
    {
        public string? Id { get; set; }
        public decimal Amount { get; set; }
        public string? TransactionContent { get; set; }
        public DateTime TransactionDate { get; set; }
        public string? BankBrandName { get; set; }
        public string? AccountNumber { get; set; }
    }
}
