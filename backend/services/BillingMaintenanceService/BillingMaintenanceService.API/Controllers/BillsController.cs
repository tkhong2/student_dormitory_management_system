using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BillsController : ControllerBase
    {
        private readonly IBillRepository _billRepository;
        public BillsController(IBillRepository billRepository)
        {
            _billRepository = billRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BillDto>>> GetAll()
        {
            var bills = await _billRepository.GetAllAsync();
            var billDtos = bills.Select(b => new BillDto
            {
                Id = b.Id,
                ContractId = b.ContractId,
                StudentId = b.StudentId,
                Amount = b.Amount,
                DueDate = b.DueDate,
                Status = b.Status.ToString(),
                Description = b.Description,
                CreatedAt = b.CreatedAt
            });
            return Ok(billDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BillDto>> GetById(Guid id)
        {
            var bill = await _billRepository.GetByIdAsync(id);
            if (bill == null) return NotFound();
            var billDto = new BillDto
            {
                Id = bill.Id,
                ContractId = bill.ContractId,
                StudentId = bill.StudentId,
                Amount = bill.Amount,
                DueDate = bill.DueDate,
                Status = bill.Status.ToString(),
                Description = bill.Description,
                CreatedAt = bill.CreatedAt
            };
            return Ok(billDto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] BillDto dto)
        {
            var bill = new Bill
            {
                Id = Guid.NewGuid(),
                ContractId = dto.ContractId,
                StudentId = dto.StudentId,
                Amount = dto.Amount,
                DueDate = dto.DueDate,
                Status = Enum.Parse<Domain.Enums.BillStatus>(dto.Status),
                Description = dto.Description,
                CreatedAt = DateTime.UtcNow
            };
            await _billRepository.AddAsync(bill);
            return CreatedAtAction(nameof(GetById), new { id = bill.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] BillDto dto)
        {
            var bill = await _billRepository.GetByIdAsync(id);
            if (bill == null) return NotFound();
            bill.ContractId = dto.ContractId;
            bill.StudentId = dto.StudentId;
            bill.Amount = dto.Amount;
            bill.DueDate = dto.DueDate;
            bill.Status = Enum.Parse<Domain.Enums.BillStatus>(dto.Status);
            bill.Description = dto.Description;
            await _billRepository.UpdateAsync(bill);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var bill = await _billRepository.GetByIdAsync(id);
            if (bill == null) return NotFound();
            await _billRepository.DeleteAsync(bill);
            return NoContent();
        }
    }
}