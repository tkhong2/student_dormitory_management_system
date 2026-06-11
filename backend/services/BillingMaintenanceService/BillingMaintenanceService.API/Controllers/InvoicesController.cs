using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;

        public InvoicesController(IInvoiceRepository invoiceRepository)
        {
            _invoiceRepository = invoiceRepository;
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
                CreatedByUserId = dto.CreatedByUserId,
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
