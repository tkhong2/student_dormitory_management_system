using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Domain.Enums;
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
        public PaymentsController(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentDto>>> GetAll()
        {
            var payments = await _paymentRepository.GetAllAsync();
            var dtos = payments.Select(p => new PaymentDto
            {
                Id = p.Id,
                BillId = p.BillId,
                StudentId = p.StudentId,
                Amount = p.Amount,
                PaymentMethod = (int)p.PaymentMethod,
                TransactionCode = p.TransactionCode,
                PaidAt = p.PaidAt,
                Note = p.Note
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentDto>> GetById(Guid id)
        {
            var p = await _paymentRepository.GetByIdAsync(id);
            if (p == null) return NotFound();
            var dto = new PaymentDto
            {
                Id = p.Id,
                BillId = p.BillId,
                StudentId = p.StudentId,
                Amount = p.Amount,
                PaymentMethod = (int)p.PaymentMethod,
                TransactionCode = p.TransactionCode,
                PaidAt = p.PaidAt,
                Note = p.Note
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] PaymentDto dto)
        {
            var p = new Payment
            {
                Id = Guid.NewGuid(),
                BillId = dto.BillId,
                StudentId = dto.StudentId,
                Amount = dto.Amount,
                PaymentMethod = (PaymentMethod)dto.PaymentMethod,
                TransactionCode = dto.TransactionCode,
                PaidAt = dto.PaidAt,
                Note = dto.Note
            };
            await _paymentRepository.AddAsync(p);
            return CreatedAtAction(nameof(GetById), new { id = p.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] PaymentDto dto)
        {
            var p = await _paymentRepository.GetByIdAsync(id);
            if (p == null) return NotFound();
            p.BillId = dto.BillId;
            p.StudentId = dto.StudentId;
            p.Amount = dto.Amount;
            p.PaymentMethod = (PaymentMethod)dto.PaymentMethod;
            p.TransactionCode = dto.TransactionCode;
            p.PaidAt = dto.PaidAt;
            p.Note = dto.Note;
            await _paymentRepository.UpdateAsync(p);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var p = await _paymentRepository.GetByIdAsync(id);
            if (p == null) return NotFound();
            await _paymentRepository.DeleteAsync(p);
            return NoContent();
        }
    }
}
