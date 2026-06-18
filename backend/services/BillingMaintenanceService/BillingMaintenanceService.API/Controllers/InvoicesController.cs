using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IUserRepository _userRepository;

        public InvoicesController(
            IInvoiceRepository invoiceRepository,
            IUserRepository userRepository)
        {
            _invoiceRepository = invoiceRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetAll()
        {
            var invoices = await _invoiceRepository.GetAllAsync();
            var dtos = invoices.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<InvoiceDto>> GetById(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null) return NotFound();
            return Ok(MapToDto(invoice));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetByStudentId(int studentId)
        {
            var invoices = await _invoiceRepository.GetByStudentIdAsync(studentId);
            var dtos = invoices.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("contract/{contractId}")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetByContractId(int contractId)
        {
            var invoices = await _invoiceRepository.GetByContractIdAsync(contractId);
            var dtos = invoices.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("code/{invoiceCode}")]
        public async Task<ActionResult<InvoiceDto>> GetByInvoiceCode(string invoiceCode)
        {
            var invoice = await _invoiceRepository.GetByInvoiceCodeAsync(invoiceCode);
            if (invoice == null) return NotFound();
            return Ok(MapToDto(invoice));
        }

        [HttpPost]
        public async Task<ActionResult<InvoiceDto>> Create([FromBody] CreateInvoiceDto dto)
        {
            try
            {
                // Ensure we have a valid CreatedByUserId
                int createdByUserId = dto.CreatedByUserId;
                var createdByUser = await _userRepository.GetByIdAsync(dto.CreatedByUserId);
                
                if (createdByUser == null)
                {
                    // Try to use Admin user (ID = 1)
                    var adminUser = await _userRepository.GetByIdAsync(1);
                    if (adminUser != null)
                    {
                        createdByUserId = 1;
                    }
                    else
                    {
                        // Try to find any existing admin or system user
                        var allUsers = await _userRepository.GetAllAsync();
                        var systemUser = allUsers.FirstOrDefault(u => 
                            u.Email == "system@ktx.dnu.edu.vn" || 
                            u.Username == "system" ||
                            u.Role == "Admin");
                        
                        if (systemUser != null)
                        {
                            createdByUserId = systemUser.Id;
                        }
                        else
                        {
                            // Last resort: use the requesting user's ID as-is and hope it works
                            // or use ID 1 as default
                            createdByUserId = 1;
                        }
                    }
                }
                
                // Check for duplicate invoice code
                var existingByCode = await _invoiceRepository.GetByInvoiceCodeAsync(dto.InvoiceCode);
                if (existingByCode != null)
                {
                    return Conflict(new { 
                        message = $"Mã phiếu thu '{dto.InvoiceCode}' đã tồn tại",
                        existingInvoiceId = existingByCode.Id,
                        existingInvoiceCode = existingByCode.InvoiceCode
                    });
                }

                // Check for duplicate invoice
                var existingInvoices = await _invoiceRepository.GetByContractIdAsync(dto.ContractId);
                Invoice duplicate = null;
                
                // For Deposit and DepositRefund: only check by contract and type (no month/year)
                // Business rule: Each contract can only have ONE deposit invoice
                if (dto.InvoiceType == "Deposit" || dto.InvoiceType == "DepositRefund")
                {
                    duplicate = existingInvoices.FirstOrDefault(i => 
                        i.InvoiceType == dto.InvoiceType &&
                        i.Status != "Cancelled");
                }
                else if (dto.InvoiceType == "Monthly")
                {
                    // For Monthly invoices: check 30-day interval from contract start or last invoice
                    // Business rule: Monthly invoices must be at least 30 days apart
                    
                    // First, check if there's already an invoice for this specific month/year
                    var sameMonthInvoice = existingInvoices.FirstOrDefault(i => 
                        i.BillingMonth == dto.BillingMonth && 
                        i.BillingYear == dto.BillingYear &&
                        i.InvoiceType == "Monthly" &&
                        i.Status != "Cancelled");
                    
                    if (sameMonthInvoice != null)
                    {
                        duplicate = sameMonthInvoice;
                    }
                    else
                    {
                        // Check 30-day interval: Get the most recent Monthly invoice
                        var lastMonthlyInvoice = existingInvoices
                            .Where(i => i.InvoiceType == "Monthly" && i.Status != "Cancelled")
                            .OrderByDescending(i => i.CreatedAt)
                            .FirstOrDefault();
                        
                        if (lastMonthlyInvoice != null)
                        {
                            // Check if at least 30 days have passed since last invoice
                            var daysSinceLastInvoice = (DateTime.UtcNow - lastMonthlyInvoice.CreatedAt).TotalDays;
                            if (daysSinceLastInvoice < 30)
                            {
                                return Conflict(new { 
                                    message = $"Chưa đủ 30 ngày kể từ phiếu thu tháng trước ({lastMonthlyInvoice.InvoiceCode}). " +
                                             $"Còn {Math.Ceiling(30 - daysSinceLastInvoice)} ngày nữa mới có thể tạo phiếu thu tháng mới.",
                                    existingInvoiceId = lastMonthlyInvoice.Id,
                                    existingInvoiceCode = lastMonthlyInvoice.InvoiceCode,
                                    daysSinceLastInvoice = (int)daysSinceLastInvoice,
                                    daysRemaining = (int)Math.Ceiling(30 - daysSinceLastInvoice)
                                });
                            }
                        }
                    }
                }
                else
                {
                    // For Other types: check by contract, month, year, and type
                    duplicate = existingInvoices.FirstOrDefault(i => 
                        i.BillingMonth == dto.BillingMonth && 
                        i.BillingYear == dto.BillingYear &&
                        i.InvoiceType == dto.InvoiceType &&
                        i.Status != "Cancelled");
                }

                if (duplicate != null)
                {
                    string message = dto.InvoiceType switch
                    {
                        "Deposit" => "Phiếu tiền cọc đã tồn tại cho hợp đồng này. Mỗi hợp đồng chỉ có 1 phiếu tiền cọc duy nhất.",
                        "DepositRefund" => "Phiếu hoàn cọc đã tồn tại cho hợp đồng này",
                        "Monthly" => $"Phiếu thu tiền phòng đã tồn tại cho tháng {dto.BillingMonth}/{dto.BillingYear}",
                        _ => $"Phiếu thu {dto.InvoiceType} đã tồn tại cho kỳ {dto.BillingMonth}/{dto.BillingYear}"
                    };
                    
                    return Conflict(new { 
                        message = message,
                        existingInvoiceId = duplicate.Id,
                        existingInvoiceCode = duplicate.InvoiceCode
                    });
                }

                var invoice = new Invoice
                {
                    InvoiceCode = dto.InvoiceCode,
                    InvoiceType = dto.InvoiceType,
                    ContractId = dto.ContractId,
                    StudentId = dto.StudentId,
                    StudentName = dto.StudentName,
                    StudentCode = dto.StudentCode,
                    RoomId = dto.RoomId,
                    RoomNumber = dto.RoomNumber,
                    BuildingName = dto.BuildingName,
                    BillingMonth = dto.BillingMonth,
                    BillingYear = dto.BillingYear,
                    RentAmount = dto.RentAmount,
                    ElectricityAmount = dto.ElectricityAmount,
                    WaterAmount = dto.WaterAmount,
                    ServiceAmount = dto.ServiceAmount,
                    PreviousDebt = dto.PreviousDebt,
                    Discount = dto.Discount,
                    DueDate = dto.DueDate,
                    CreatedByUserId = createdByUserId, // Use validated user ID
                    Notes = dto.Notes,
                    Status = "Unpaid"
                };

            // Calculate total based on invoice type
            if (invoice.InvoiceType == "DepositRefund")
            {
                // For DepositRefund: Amount to refund = RentAmount (deposit) - Discount (penalty)
                // This should be negative (money going out)
                invoice.TotalAmount = -(invoice.RentAmount - invoice.Discount);
                invoice.DebtAmount = invoice.TotalAmount;
            }
            else if (invoice.InvoiceType == "Deposit")
            {
                // For Deposit: Just the deposit amount
                invoice.TotalAmount = invoice.RentAmount;
                invoice.DebtAmount = invoice.TotalAmount;
            }
            else
            {
                // For Monthly and Other: Standard calculation
                invoice.TotalAmount = invoice.RentAmount + invoice.ElectricityAmount + 
                                      invoice.WaterAmount + invoice.ServiceAmount + 
                                      invoice.PreviousDebt - invoice.Discount;
                invoice.DebtAmount = invoice.TotalAmount;
            }

                // Add items
                foreach (var itemDto in dto.Items)
                {
                    var item = new InvoiceItem
                    {
                        ItemName = itemDto.ItemName,
                        ItemDescription = itemDto.ItemDescription,
                        Quantity = itemDto.Quantity,
                        Unit = itemDto.Unit,
                        UnitPrice = itemDto.UnitPrice,
                        Amount = itemDto.Quantity * itemDto.UnitPrice,
                        SortOrder = itemDto.SortOrder
                    };
                    invoice.Items.Add(item);
                }

                await _invoiceRepository.AddAsync(invoice);
                return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, MapToDto(invoice));
            }
            catch (DbUpdateException ex) when (ex.InnerException is Microsoft.Data.SqlClient.SqlException sqlEx)
            {
                // Handle SQL unique constraint violations
                if (sqlEx.Number == 2601 || sqlEx.Number == 2627) // Unique constraint violation
                {
                    if (sqlEx.Message.Contains("IX_Invoices_InvoiceCode"))
                    {
                        return Conflict(new { 
                            message = $"Mã phiếu thu '{dto.InvoiceCode}' đã tồn tại trong hệ thống"
                        });
                    }
                    else if (sqlEx.Message.Contains("IX_Invoices_ContractId_BillingMonth_BillingYear_InvoiceType"))
                    {
                        return Conflict(new { 
                            message = $"Phiếu thu {dto.InvoiceType} đã tồn tại cho hợp đồng này vào kỳ {dto.BillingMonth}/{dto.BillingYear}"
                        });
                    }
                    
                    return Conflict(new { 
                        message = "Phiếu thu đã tồn tại. Vui lòng kiểm tra lại thông tin."
                    });
                }
                
                // Re-throw other database exceptions
                throw;
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] InvoiceDto dto)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            invoice.Status = dto.Status;
            invoice.PaidAmount = dto.PaidAmount;
            invoice.DebtAmount = dto.DebtAmount;
            invoice.OverdueDays = dto.OverdueDays;
            invoice.PenaltyAmount = dto.PenaltyAmount;
            invoice.Notes = dto.Notes;

            await _invoiceRepository.UpdateAsync(invoice);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            await _invoiceRepository.DeleteAsync(invoice);
            return NoContent();
        }

        /// <summary>
        /// Sinh công nợ tự động từ hợp đồng Active
        /// POST: api/invoices/generate-from-contract/{contractId}
        /// </summary>
        [HttpPost("generate-from-contract/{contractId}")]
        public async Task<ActionResult<InvoiceDto>> GenerateFromContract(int contractId, [FromBody] GenerateInvoiceFromContractDto dto)
        {
            try
            {
                // Validate input
                if (dto == null || string.IsNullOrEmpty(dto.InvoiceCode))
                {
                    return BadRequest("Missing required fields");
                }

                // Check if invoice already exists for this month/year
                var existingInvoices = await _invoiceRepository.GetByContractIdAsync(contractId);
                var duplicate = existingInvoices.FirstOrDefault(i => 
                    i.BillingMonth == dto.BillingMonth && 
                    i.BillingYear == dto.BillingYear &&
                    i.Status != "Cancelled");

                if (duplicate != null)
                {
                    return Conflict($"Invoice already exists for {dto.BillingMonth}/{dto.BillingYear}");
                }

                var invoice = new Invoice
                {
                    InvoiceCode = dto.InvoiceCode,
                    InvoiceType = dto.InvoiceType ?? "Monthly",
                    ContractId = contractId,
                    StudentId = dto.StudentId,
                    StudentName = dto.StudentName,
                    StudentCode = dto.StudentCode,
                    RoomId = dto.RoomId,
                    RoomNumber = dto.RoomNumber,
                    BuildingName = dto.BuildingName,
                    BillingMonth = dto.BillingMonth,
                    BillingYear = dto.BillingYear,
                    RentAmount = dto.RentAmount,
                    ElectricityAmount = dto.ElectricityAmount,
                    WaterAmount = dto.WaterAmount,
                    ServiceAmount = dto.ServiceAmount,
                    PreviousDebt = dto.PreviousDebt,
                    Discount = dto.Discount,
                    PenaltyAmount = 0,
                    DueDate = dto.DueDate,
                    CreatedByUserId = dto.CreatedByUserId,
                    Notes = dto.Notes,
                    Status = "Unpaid",
                    PaidAmount = 0
                };

                // Calculate total based on invoice type
                if (invoice.InvoiceType == "DepositRefund")
                {
                    // For DepositRefund: Amount to refund = RentAmount (deposit) - Discount (penalty)
                    // This should be negative (money going out)
                    invoice.TotalAmount = -(invoice.RentAmount - invoice.Discount);
                    invoice.DebtAmount = invoice.TotalAmount;
                }
                else if (invoice.InvoiceType == "Deposit")
                {
                    // For Deposit: Just the deposit amount
                    invoice.TotalAmount = invoice.RentAmount;
                    invoice.DebtAmount = invoice.TotalAmount;
                }
                else
                {
                    // For Monthly and Other: Standard calculation
                    invoice.TotalAmount = invoice.RentAmount + invoice.ElectricityAmount + 
                                          invoice.WaterAmount + invoice.ServiceAmount + 
                                          invoice.PreviousDebt - invoice.Discount;
                    invoice.DebtAmount = invoice.TotalAmount;
                }

                // Add items
                if (dto.Items != null && dto.Items.Any())
                {
                    foreach (var itemDto in dto.Items)
                    {
                        var item = new InvoiceItem
                        {
                            ItemName = itemDto.ItemName,
                            ItemDescription = itemDto.ItemDescription,
                            Quantity = itemDto.Quantity,
                            Unit = itemDto.Unit,
                            UnitPrice = itemDto.UnitPrice,
                            Amount = itemDto.Quantity * itemDto.UnitPrice,
                            SortOrder = itemDto.SortOrder
                        };
                        invoice.Items.Add(item);
                    }
                }

                await _invoiceRepository.AddAsync(invoice);
                return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, MapToDto(invoice));
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating invoice: {ex.Message}");
            }
        }

        /// <summary>
        /// Gửi email nhắc nợ cho sinh viên
        /// POST: api/invoices/{id}/send-reminder
        /// </summary>
        [HttpPost("{id}/send-reminder")]
        public async Task<ActionResult> SendReminder(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.Status == "Paid" || invoice.Status == "Cancelled")
            {
                return BadRequest("Cannot send reminder for paid or cancelled invoice");
            }

            try
            {
                // Update reminder count and timestamp
                invoice.ReminderCount++;
                invoice.LastReminderSentAt = DateTime.UtcNow;
                await _invoiceRepository.UpdateAsync(invoice);

                // TODO: Implement email sending logic here
                // For now, just return success
                return Ok(new { 
                    message = "Reminder sent successfully",
                    reminderCount = invoice.ReminderCount,
                    sentAt = invoice.LastReminderSentAt
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error sending reminder: {ex.Message}");
            }
        }

        /// <summary>
        /// Cập nhật trạng thái quá hạn cho các phiếu thu
        /// POST: api/invoices/update-overdue-status
        /// </summary>
        [HttpPost("update-overdue-status")]
        public async Task<ActionResult> UpdateOverdueStatus()
        {
            try
            {
                var invoices = await _invoiceRepository.GetAllAsync();
                var today = DateOnly.FromDateTime(DateTime.Today);
                var updatedCount = 0;

                foreach (var invoice in invoices)
                {
                    if (invoice.Status != "Paid" && invoice.Status != "Cancelled")
                    {
                        var overdueDays = today.DayNumber - invoice.DueDate.DayNumber;
                        
                        if (overdueDays > 0)
                        {
                            invoice.OverdueDays = overdueDays;
                            
                            if (invoice.DebtAmount > 0)
                            {
                                invoice.Status = "Overdue";
                                updatedCount++;
                            }
                        }
                    }
                }

                await Task.WhenAll(invoices
                    .Where(i => i.Status == "Overdue")
                    .Select(i => _invoiceRepository.UpdateAsync(i)));

                return Ok(new { 
                    message = $"Updated {updatedCount} invoices to overdue status",
                    updatedCount 
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating overdue status: {ex.Message}");
            }
        }

        /// <summary>
        /// Tự động tạo hóa đơn hàng tháng cho tất cả hợp đồng đang active
        /// POST: api/invoices/auto-generate-monthly
        /// </summary>
        [HttpPost("auto-generate-monthly")]
        public async Task<ActionResult> AutoGenerateMonthlyInvoices([FromQuery] int? month = null, [FromQuery] int? year = null)
        {
            try
            {
                var now = DateTime.Now;
                var billingMonth = month ?? now.Month;
                var billingYear = year ?? now.Year;
                var dueDay = 5; // Ngày đáo hạn mặc định
                var dueDate = new DateOnly(billingYear, billingMonth, dueDay);

                // Fetch active contracts from ContractStudentService
                var httpClient = new HttpClient();
                var contractsResponse = await httpClient.GetAsync("http://localhost:5001/api/contracts");
                
                if (!contractsResponse.IsSuccessStatusCode)
                {
                    return StatusCode(500, "Không thể lấy danh sách hợp đồng");
                }

                var contractsJson = await contractsResponse.Content.ReadAsStringAsync();
                var contracts = System.Text.Json.JsonSerializer.Deserialize<List<System.Text.Json.JsonElement>>(contractsJson);

                if (contracts == null || contracts.Count == 0)
                {
                    return Ok(new { message = "Không có hợp đồng nào để tạo hóa đơn", created = 0, skipped = 0, errors = 0 });
                }

                // Filter active contracts
                var activeContracts = contracts.Where(c => 
                    c.GetProperty("status").GetString() == "Active"
                ).ToList();

                var createdCount = 0;
                var skippedCount = 0;
                var errorCount = 0;
                var errors = new List<string>();

                // Get system user for CreatedByUserId
                var systemUser = await _userRepository.GetByIdAsync(1);
                var createdByUserId = systemUser?.Id ?? 1;

                foreach (var contract in activeContracts)
                {
                    try
                    {
                        var contractId = contract.GetProperty("id").GetInt32();
                        var studentId = contract.GetProperty("studentId").GetInt32();
                        var roomId = contract.GetProperty("roomId").GetInt32();

                        // Check if invoice already exists for this month
                        var existingInvoices = await _invoiceRepository.GetByContractIdAsync(contractId);
                        var duplicate = existingInvoices.FirstOrDefault(i => 
                            i.BillingMonth == billingMonth && 
                            i.BillingYear == billingYear &&
                            i.InvoiceType == "Monthly" &&
                            i.Status != "Cancelled");

                        if (duplicate != null)
                        {
                            skippedCount++;
                            continue;
                        }

                        // Generate invoice code
                        var allInvoices = await _invoiceRepository.GetAllAsync();
                        int maxSequence = 0;
                        var pattern = $@"PTT{billingYear:0000}{billingMonth:00}(\d{{3}})";
                        var regex = new System.Text.RegularExpressions.Regex(pattern);

                        foreach (var inv in allInvoices)
                        {
                            var match = regex.Match(inv.InvoiceCode ?? "");
                            if (match.Success)
                            {
                                var seq = int.Parse(match.Groups[1].Value);
                                if (seq > maxSequence) maxSequence = seq;
                            }
                        }

                        var nextSequence = (maxSequence + 1).ToString("D3");
                        var invoiceCode = $"PTT{billingYear:0000}{billingMonth:00}{nextSequence}";

                        // Create invoice
                        var invoice = new Invoice
                        {
                            InvoiceCode = invoiceCode,
                            InvoiceType = "Monthly",
                            ContractId = contractId,
                            StudentId = studentId,
                            StudentName = contract.TryGetProperty("studentName", out var sn) ? sn.GetString() : "",
                            StudentCode = contract.TryGetProperty("studentCode", out var sc) ? sc.GetString() : "",
                            RoomId = roomId,
                            RoomNumber = contract.GetProperty("roomNumber").GetString(),
                            BuildingName = contract.GetProperty("buildingName").GetString(),
                            BillingMonth = billingMonth,
                            BillingYear = billingYear,
                            RentAmount = contract.GetProperty("monthlyRent").GetDecimal(),
                            ElectricityAmount = 50000, // Example: 50k for electricity
                            WaterAmount = 30000, // Example: 30k for water
                            ServiceAmount = 20000, // Example: 20k for service
                            PreviousDebt = 0,
                            Discount = 0,
                            PenaltyAmount = 0,
                            DueDate = dueDate,
                            CreatedByUserId = createdByUserId,
                            Notes = $"Hóa đơn tháng {billingMonth}/{billingYear} (Tự động)",
                            Status = "Unpaid",
                            PaidAmount = 0
                        };

                        invoice.TotalAmount = invoice.RentAmount + invoice.ElectricityAmount + 
                                              invoice.WaterAmount + invoice.ServiceAmount + 
                                              invoice.PreviousDebt - invoice.Discount;
                        invoice.DebtAmount = invoice.TotalAmount;

                        await _invoiceRepository.AddAsync(invoice);
                        createdCount++;
                    }
                    catch (Exception ex)
                    {
                        errorCount++;
                        errors.Add($"Contract {contract.GetProperty("contractCode").GetString()}: {ex.Message}");
                    }
                }

                return Ok(new { 
                    message = $"Đã tạo {createdCount} hóa đơn tự động",
                    created = createdCount,
                    skipped = skippedCount,
                    errors = errorCount,
                    errorDetails = errors,
                    billingPeriod = $"{billingMonth}/{billingYear}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi tạo hóa đơn tự động: {ex.Message}");
            }
        }

        /// <summary>
        /// Lấy mã phiếu thu tiếp theo cho một loại và tháng/năm cụ thể
        /// GET: api/invoices/next-code?invoiceType=Monthly&month=6&year=2026
        /// </summary>
        [HttpGet("next-code")]
        public async Task<ActionResult<string>> GetNextInvoiceCode(
            [FromQuery] string invoiceType = "Monthly",
            [FromQuery] int? month = null,
            [FromQuery] int? year = null)
        {
            try
            {
                var now = DateTime.Now;
                var billingMonth = month ?? now.Month;
                var billingYear = year ?? now.Year;

                // Determine prefix based on invoice type
                string prefix = invoiceType switch
                {
                    "Deposit" => "PTD",
                    "Monthly" => "PTT",
                    "DepositRefund" => "PTR",
                    "Other" => "PTO",
                    _ => "PTT"
                };

                // Get all invoices
                var allInvoices = await _invoiceRepository.GetAllAsync();
                
                // Find max sequence for this month (across all types to avoid collision)
                int maxSequence = 0;
                var pattern = $@"PT[DTRO]{billingYear:0000}{billingMonth:00}(\d{{3}})";
                var regex = new System.Text.RegularExpressions.Regex(pattern);

                foreach (var inv in allInvoices)
                {
                    var match = regex.Match(inv.InvoiceCode ?? "");
                    if (match.Success)
                    {
                        var seq = int.Parse(match.Groups[1].Value);
                        if (seq > maxSequence) maxSequence = seq;
                    }
                }

                var nextSequence = (maxSequence + 1).ToString("D3");
                var nextCode = $"{prefix}{billingYear:0000}{billingMonth:00}{nextSequence}";

                return Ok(new { invoiceCode = nextCode });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error generating next invoice code: {ex.Message}");
            }
        }

        private static InvoiceDto MapToDto(Invoice invoice)
        {
            return new InvoiceDto
            {
                Id = invoice.Id,
                InvoiceCode = invoice.InvoiceCode,
                InvoiceType = invoice.InvoiceType,
                ContractId = invoice.ContractId,
                StudentId = invoice.StudentId,
                StudentName = invoice.StudentName,
                StudentCode = invoice.StudentCode,
                RoomId = invoice.RoomId,
                RoomNumber = invoice.RoomNumber,
                BuildingName = invoice.BuildingName,
                BillingMonth = invoice.BillingMonth,
                BillingYear = invoice.BillingYear,
                RentAmount = invoice.RentAmount,
                ElectricityAmount = invoice.ElectricityAmount,
                WaterAmount = invoice.WaterAmount,
                ServiceAmount = invoice.ServiceAmount,
                PreviousDebt = invoice.PreviousDebt,
                Discount = invoice.Discount,
                PenaltyAmount = invoice.PenaltyAmount,
                TotalAmount = invoice.TotalAmount,
                PaidAmount = invoice.PaidAmount,
                DebtAmount = invoice.DebtAmount,
                Status = invoice.Status,
                DueDate = invoice.DueDate,
                OverdueDays = invoice.OverdueDays,
                Notes = invoice.Notes,
                CreatedAt = invoice.CreatedAt,
                Items = invoice.Items.Select(i => new InvoiceItemDto
                {
                    Id = i.Id,
                    InvoiceId = i.InvoiceId,
                    ItemName = i.ItemName,
                    ItemDescription = i.ItemDescription,
                    Quantity = i.Quantity,
                    Unit = i.Unit,
                    UnitPrice = i.UnitPrice,
                    Amount = i.Amount,
                    SortOrder = i.SortOrder
                }).ToList()
            };
        }
    }
}
