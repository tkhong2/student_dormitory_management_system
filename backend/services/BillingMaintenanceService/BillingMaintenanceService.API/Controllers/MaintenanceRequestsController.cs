using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MaintenanceRequestsController : ControllerBase
    {
        private readonly IMaintenanceRequestRepository _maintenanceRequestRepository;
        public MaintenanceRequestsController(IMaintenanceRequestRepository maintenanceRequestRepository)
        {
            _maintenanceRequestRepository = maintenanceRequestRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetAll()
        {
            var requests = await _maintenanceRequestRepository.GetAllAsync();
            var dtos = requests.Select(r => new MaintenanceRequestDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                StudentId = r.StudentId,
                Description = r.Description,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt,
                Note = r.Note
            });
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceRequestDto>> GetById(Guid id)
        {
            var r = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (r == null) return NotFound();
            var dto = new MaintenanceRequestDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                StudentId = r.StudentId,
                Description = r.Description,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt,
                Note = r.Note
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] MaintenanceRequestDto dto)
        {
            var r = new MaintenanceRequest
            {
                Id = Guid.NewGuid(),
                RoomId = dto.RoomId,
                StudentId = dto.StudentId,
                Description = dto.Description,
                Status = Enum.Parse<Domain.Enums.MaintenanceStatus>(dto.Status),
                CreatedAt = DateTime.UtcNow,
                Note = dto.Note
            };
            await _maintenanceRequestRepository.AddAsync(r);
            return CreatedAtAction(nameof(GetById), new { id = r.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] MaintenanceRequestDto dto)
        {
            var r = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (r == null) return NotFound();
            r.RoomId = dto.RoomId;
            r.StudentId = dto.StudentId;
            r.Description = dto.Description;
            r.Status = Enum.Parse<Domain.Enums.MaintenanceStatus>(dto.Status);
            r.Note = dto.Note;
            await _maintenanceRequestRepository.UpdateAsync(r);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var r = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (r == null) return NotFound();
            await _maintenanceRequestRepository.DeleteAsync(r);
            return NoContent();
        }
    }
}