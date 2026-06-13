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
        /// Webhook endpoint to receive payment notifications from Sepay
        /// </summary>
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook([FromBody] SepayWebhookDto webhookData)
        {
            try
            {
                _logger.LogInformation("Received Sepay webhook: {Data}", JsonSerializer.Serialize(webhookData));

                // Validate webhook (optional: check signature if Sepay provides one)
                
                // Parse transfer content to get invoice code
                var transferContent = webhookData.TransferContent ?? webhookData.Description ?? "";
                _logger.LogInformation("Transfer content: {Content}", transferContent);

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
                var amountReceived = webhookData.Amount;
                
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
                    PaymentDate = webhookData.TransactionDate.HasValue 
                        ? DateOnly.FromDateTime(webhookData.TransactionDate.Value) 
                        : DateOnly.FromDateTime(DateTime.Now),
                    PaidAt = DateTime.Now,
                    TransactionCode = webhookData.TransactionId ?? webhookData.ReferenceNumber,
                    BankName = "BIDV",
                    BankAccountNumber = webhookData.AccountNumber,
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
                                    TransactionId = matchingTransaction.Id,
                                    Amount = matchingTransaction.Amount,
                                    TransferContent = matchingTransaction.TransactionContent,
                                    TransactionDate = matchingTransaction.TransactionDate,
                                    AccountNumber = sepayAccountNumber,
                                    BankCode = matchingTransaction.BankBrandName
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
                _logger.LogInformation("🧪 TEST: Simulating payment for invoice: {Code}", dto.InvoiceCode);

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

                // Simulate webhook data
                var webhookData = new SepayWebhookDto
                {
                    TransactionId = $"TEST{DateTime.Now:yyyyMMddHHmmss}",
                    Amount = dto.Amount > 0 ? dto.Amount : invoice.DebtAmount,
                    TransferContent = $"{invoice.InvoiceCode} {invoice.StudentCode}",
                    TransactionDate = DateTime.Now,
                    AccountNumber = "8871422018",
                    BankCode = "970418"
                };

                // Call webhook method
                var result = await Webhook(webhookData);
                
                _logger.LogInformation("🧪 TEST: Payment simulation completed for invoice: {Code}", dto.InvoiceCode);

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

            // Look for pattern PTT followed by numbers (e.g., PTT202606001)
            var match = System.Text.RegularExpressions.Regex.Match(content, @"PTT\d+");
            return match.Success ? match.Value : "";
        }
    }

    // DTOs for Sepay webhook
    public class SepayWebhookDto
    {
        public string? TransactionId { get; set; }
        public string? ReferenceNumber { get; set; }
        public decimal Amount { get; set; }
        public string? TransferContent { get; set; }
        public string? Description { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? AccountNumber { get; set; }
        public string? BankCode { get; set; }
    }

    public class CheckPaymentDto
    {
        public string InvoiceCode { get; set; } = null!;
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
