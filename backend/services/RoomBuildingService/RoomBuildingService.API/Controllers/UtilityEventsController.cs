using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityEventsController : ControllerBase
    {
        private readonly IUtilityEventRepository _repository;
        private readonly ISharedUtilityRepository _utilityRepository;

        public UtilityEventsController(IUtilityEventRepository repository, ISharedUtilityRepository utilityRepository)
        {
            _repository = repository;
            _utilityRepository = utilityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilityEventDto>>> GetAll()
        {
            var events = await _repository.GetAllAsync();
            return Ok(events.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UtilityEventDto>> GetById(int id)
        {
            var evt = await _repository.GetByIdAsync(id);
            if (evt == null) return NotFound();
            return Ok(MapToDto(evt));
        }

        [HttpGet("utility/{utilityId}")]
        public async Task<ActionResult<IEnumerable<UtilityEventDto>>> GetByUtility(int utilityId)
        {
            var events = await _repository.GetBySharedUtilityIdAsync(utilityId);
            return Ok(events.Select(MapToDto));
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<UtilityEventDto>>> GetByStatus(string status)
        {
            var events = await _repository.GetByStatusAsync(status);
            return Ok(events.Select(MapToDto));
        }

        [HttpGet("upcoming-maintenance")]
        public async Task<ActionResult<IEnumerable<UtilityEventDto>>> GetUpcomingMaintenance()
        {
            var events = await _repository.GetUpcomingMaintenanceAsync();
            return Ok(events.Select(MapToDto));
        }

        [HttpPost]
        public async Task<ActionResult<UtilityEventDto>> Create([FromBody] CreateUtilityEventDto dto)
        {
            var evt = new UtilityEvent
            {
                SharedUtilityId = dto.SharedUtilityId,
                EventType = dto.EventType,
                Title = dto.Title,
                Description = dto.Description,
                EventDate = dto.EventDate,
                EstimatedCost = dto.EstimatedCost,
                PerformedByUserId = dto.PerformedByUserId,
                PerformedByName = dto.PerformedByName,
                TechnicianCompany = dto.TechnicianCompany,
                TechnicianContact = dto.TechnicianContact,
                MaintenanceRequestId = dto.MaintenanceRequestId,
                Notes = dto.Notes,
                Status = "Scheduled"
            };

            await _repository.AddAsync(evt);

            // Update utility's next maintenance date if this is a maintenance event
            if (dto.EventType == "Maintenance" || dto.EventType == "Inspection")
            {
                var utility = await _utilityRepository.GetByIdAsync(dto.SharedUtilityId);
                if (utility != null)
                {
                    utility.NextMaintenanceDate = dto.EventDate;
                    await _utilityRepository.UpdateAsync(utility);
                }
            }

            return CreatedAtAction(nameof(GetById), new { id = evt.Id }, MapToDto(evt));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateUtilityEventDto dto)
        {
            var evt = await _repository.GetByIdAsync(id);
            if (evt == null) return NotFound();

            if (dto.Status != null) evt.Status = dto.Status;
            if (dto.CompletedDate.HasValue) evt.CompletedDate = dto.CompletedDate;
            if (dto.ActualCost.HasValue) evt.ActualCost = dto.ActualCost;
            if (dto.Notes != null) evt.Notes = dto.Notes;
            if (dto.ImageUrls != null) evt.ImageUrls = dto.ImageUrls;

            await _repository.UpdateAsync(evt);

            // Update utility's last maintenance date if event is completed
            if (dto.Status == "Completed" && (evt.EventType == "Maintenance" || evt.EventType == "Inspection"))
            {
                var utility = await _utilityRepository.GetByIdAsync(evt.SharedUtilityId);
                if (utility != null)
                {
                    utility.LastMaintenanceDate = evt.CompletedDate ?? DateTime.UtcNow;
                    await _utilityRepository.UpdateAsync(utility);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var evt = await _repository.GetByIdAsync(id);
            if (evt == null) return NotFound();

            await _repository.DeleteAsync(evt);
            return NoContent();
        }

        private UtilityEventDto MapToDto(UtilityEvent evt)
        {
            return new UtilityEventDto
            {
                Id = evt.Id,
                SharedUtilityId = evt.SharedUtilityId,
                UtilityName = evt.SharedUtility?.Name,
                EventType = evt.EventType,
                Title = evt.Title,
                Description = evt.Description,
                EventDate = evt.EventDate,
                CompletedDate = evt.CompletedDate,
                Status = evt.Status,
                EstimatedCost = evt.EstimatedCost,
                ActualCost = evt.ActualCost,
                PerformedByUserId = evt.PerformedByUserId,
                PerformedByName = evt.PerformedByName,
                TechnicianCompany = evt.TechnicianCompany,
                TechnicianContact = evt.TechnicianContact,
                MaintenanceRequestId = evt.MaintenanceRequestId,
                Notes = evt.Notes,
                ImageUrls = evt.ImageUrls,
                CreatedAt = evt.CreatedAt
            };
        }
    }
}
