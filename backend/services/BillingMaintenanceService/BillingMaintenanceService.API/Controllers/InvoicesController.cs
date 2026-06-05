using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InvoicesController : ControllerBase
    {
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IInvoiceService _invoiceService;

        public InvoicesController(IInvoiceRepository invoiceRepository, IInvoiceService invoiceService)
        {
            _invoiceRepository = invoiceRepository;
            _invoiceService = invoiceService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
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

            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
            if (role == "Student")
            {
                var refIdStr = User.FindFirstValue("referenceId");
                if (!int.TryParse(refIdStr, out var refId) || refId != invoice.StudentId)
                {
                    return Forbid();
                }
            }

            return Ok(MapToDto(invoice));
        }

        [HttpGet("student/{studentId}")]
        [Authorize(Roles = "Admin,Student")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetByStudentId(int studentId)
        {
            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
            if (role == "Student")
            {
                var refIdStr = User.FindFirstValue("referenceId");
                if (!int.TryParse(refIdStr, out var refId) || refId != studentId)
                {
                    return Forbid();
                }
            }

            var invoices = await _invoiceRepository.GetByStudentIdAsync(studentId);
            var dtos = invoices.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("contract/{contractId}")]
        [Authorize(Roles = "Admin,Staff")] // Restrict to Admin and Staff for now, unless Student needs it later
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GetByContractId(int contractId)
        {
            var invoices = await _invoiceRepository.GetByContractIdAsync(contractId);
            var dtos = invoices.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("contract/{contractId}/debt")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<object>> GetContractDebt(int contractId)
        {
            var invoices = await _invoiceRepository.GetByContractIdAsync(contractId);
            var totalDebt = invoices.Where(i => i.Status != "Paid" && i.Status != "Cancelled").Sum(i => i.DebtAmount);
            return Ok(new { contractId, totalDebt, unpaidInvoiceCount = invoices.Count(i => i.Status != "Paid" && i.Status != "Cancelled") });
        }

        [HttpGet("code/{invoiceCode}")]
        [Authorize(Roles = "Admin,Staff,Student")]
        public async Task<ActionResult<InvoiceDto>> GetByInvoiceCode(string invoiceCode)
        {
            var invoice = await _invoiceRepository.GetByInvoiceCodeAsync(invoiceCode);
            if (invoice == null) return NotFound();

            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
            if (role == "Student")
            {
                var refIdStr = User.FindFirstValue("referenceId");
                if (!int.TryParse(refIdStr, out var refId) || refId != invoice.StudentId)
                {
                    return Forbid();
                }
            }

            return Ok(MapToDto(invoice));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
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

            // Calculate total
            invoice.TotalAmount = invoice.RentAmount + invoice.ElectricityAmount + 
                                  invoice.WaterAmount + invoice.ServiceAmount + 
                                  invoice.PreviousDebt - invoice.Discount;
            invoice.DebtAmount = invoice.TotalAmount;

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

        [HttpPost("generate-monthly")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<InvoiceDto>> GenerateMonthlyInvoice([FromBody] GenerateMonthlyInvoiceRequestDto request)
        {
            var invoice = await _invoiceService.GenerateMonthlyInvoiceAsync(request.ContractId, request.BillingMonth, request.BillingYear, request.CreatedByUserId);
            return CreatedAtAction(nameof(GetById), new { id = invoice.Id }, invoice);
        }

        [HttpPost("generate-monthly-batch")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<InvoiceDto>>> GenerateMonthlyInvoices([FromBody] GenerateMonthlyInvoicesRequestDto request)
        {
            var invoices = await _invoiceService.GenerateMonthlyInvoicesForMonthAsync(request.BillingMonth, request.BillingYear, request.CreatedByUserId);
            return Ok(invoices);
        }

        [HttpPost("process-overdue")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<object>> ProcessOverdueInvoices([FromQuery] int reminderIntervalDays = 7)
        {
            var updatedCount = await _invoiceService.ProcessOverdueInvoicesAsync(reminderIntervalDays);
            return Ok(new { updatedCount });
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            await _invoiceRepository.DeleteAsync(invoice);
            return NoContent();
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