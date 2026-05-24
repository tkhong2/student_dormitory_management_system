using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                TransactionId = p.TransactionId
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
                Amount = p.Amount,
                PaymentDate = p.PaymentDate,
                PaymentMethod = p.PaymentMethod,
                TransactionId = p.TransactionId
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
                Amount = dto.Amount,
                PaymentDate = dto.PaymentDate,
                PaymentMethod = dto.PaymentMethod,
                TransactionId = dto.TransactionId
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
            p.Amount = dto.Amount;
            p.PaymentDate = dto.PaymentDate;
            p.PaymentMethod = dto.PaymentMethod;
            p.TransactionId = dto.TransactionId;
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