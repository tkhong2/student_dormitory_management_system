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
                RequestCode = r.RequestCode,
                RoomId = r.RoomId,
                StudentId = r.StudentId,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
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
                RequestCode = r.RequestCode,
                RoomId = r.RoomId,
                StudentId = r.StudentId,
                Title = r.Title,
                Description = r.Description,
                Status = r.Status.ToString(),
                CreatedAt = r.CreatedAt,
                UpdatedAt = r.UpdatedAt
            };
            return Ok(dto);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] MaintenanceRequestDto dto)
        {
            var r = new MaintenanceRequest
            {
                Id = Guid.NewGuid(),
                RequestCode = dto.RequestCode,
                RoomId = dto.RoomId,
                StudentId = dto.StudentId,
                Title = dto.Title,
                Description = dto.Description,
                Status = Enum.Parse<Domain.Enums.MaintenanceStatus>(dto.Status),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _maintenanceRequestRepository.AddAsync(r);
            return CreatedAtAction(nameof(GetById), new { id = r.Id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] MaintenanceRequestDto dto)
        {
            var r = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (r == null) return NotFound();
            r.RequestCode = dto.RequestCode;
            r.RoomId = dto.RoomId;
            r.StudentId = dto.StudentId;
            r.Title = dto.Title;
            r.Description = dto.Description;
            r.Status = Enum.Parse<Domain.Enums.MaintenanceStatus>(dto.Status);
            r.UpdatedAt = DateTime.UtcNow;
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