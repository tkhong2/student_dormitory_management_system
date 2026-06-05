using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentRepository paymentRepository, IPaymentService paymentService)
        {
            _paymentRepository = paymentRepository;
            _paymentService = paymentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
        {
            var payments = await _paymentRepository.GetAllAsync();
            return Ok(payments.Select(MapToDto));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<PaymentDto>> GetById(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) return NotFound();
            return Ok(MapToDto(payment));
        }

        [HttpGet("invoice/{invoiceId:int}")]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetByInvoiceId(int invoiceId)
        {
            var payments = await _paymentRepository.GetByInvoiceIdAsync(invoiceId);
            return Ok(payments.Select(MapToDto));
        }

        [HttpGet("student/{studentId:int}/debt")]
        public async Task<ActionResult<object>> GetTotalDebtByStudentId(int studentId)
        {
            var totalDebt = await _paymentService.GetTotalDebtByStudentIdAsync(studentId);
            return Ok(new { studentId, totalDebt });
        }

        [HttpGet("contract/{contractId:int}/debt")]
        public async Task<ActionResult<object>> GetTotalDebtByContractId(int contractId)
        {
            var totalDebt = await _paymentService.GetTotalDebtByContractIdAsync(contractId);
            return Ok(new { contractId, totalDebt });
        }

        [HttpPost]
        public async Task<ActionResult<PaymentDto>> Create([FromBody] PaymentDto dto)
        {
            var payment = await _paymentService.ProcessPaymentAsync(
                dto.InvoiceId,
                dto.Amount,
                dto.Method,
                dto.Note,
                dto.ReceivedByUserId,
                dto.ReceivedByName);

            return CreatedAtAction(nameof(GetById), new { id = payment.Id }, payment);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] PaymentDto dto)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) return NotFound();

            payment.InvoiceId = dto.InvoiceId;
            payment.Amount = dto.Amount;
            payment.Method = dto.Method;
            payment.TransactionCode = dto.TransactionCode;
            payment.BankName = dto.BankName;
            payment.BankAccountNumber = dto.BankAccountNumber;
            payment.PaymentDate = dto.PaymentDate;
            payment.PaidAt = dto.PaidAt;
            payment.ReceivedByUserId = dto.ReceivedByUserId;
            payment.ReceivedByName = dto.ReceivedByName;
            payment.ReceiptImageUrl = dto.ReceiptImageUrl;
            payment.Note = dto.Note;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var payment = await _paymentRepository.GetByIdAsync(id);
            if (payment == null) return NotFound();

            await _paymentRepository.DeleteAsync(payment);
            return NoContent();
        }

        private static PaymentDto MapToDto(Payment payment)
        {
            return new PaymentDto
            {
                Id = payment.Id,
                InvoiceId = payment.InvoiceId,
                Amount = payment.Amount,
                Method = payment.Method,
                TransactionCode = payment.TransactionCode,
                BankName = payment.BankName,
                BankAccountNumber = payment.BankAccountNumber,
                PaymentDate = payment.PaymentDate,
                PaidAt = payment.PaidAt,
                ReceivedByUserId = payment.ReceivedByUserId,
                ReceivedByName = payment.ReceivedByName,
                ReceiptImageUrl = payment.ReceiptImageUrl,
                Note = payment.Note
            };
        }
    }
}
