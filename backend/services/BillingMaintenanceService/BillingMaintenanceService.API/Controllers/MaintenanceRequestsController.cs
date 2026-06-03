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
            var dtos = requests.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MaintenanceRequestDto>> GetById(int id)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();
            return Ok(MapToDto(request));
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetByRoomId(int roomId)
        {
            var requests = await _maintenanceRequestRepository.GetByRoomIdAsync(roomId);
            var dtos = requests.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetByStudentId(int studentId)
        {
            var requests = await _maintenanceRequestRepository.GetByStudentIdAsync(studentId);
            var dtos = requests.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<MaintenanceRequestDto>> Create([FromBody] CreateMaintenanceRequestDto dto)
        {
            var request = new MaintenanceRequest
            {
                RoomId = dto.RoomId,
                RoomNumber = dto.RoomNumber,
                BuildingName = dto.BuildingName,
                RequestedByStudentId = dto.RequestedByStudentId,
                RequestedByStudentName = dto.RequestedByStudentName,
                Title = dto.Title,
                Description = dto.Description,
                Category = dto.Category,
                Priority = dto.Priority,
                ImageUrls = dto.ImageUrls,
                Status = "New"
            };

            await _maintenanceRequestRepository.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, MapToDto(request));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] MaintenanceRequestDto dto)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            request.Title = dto.Title;
            request.Description = dto.Description;
            request.Category = dto.Category;
            request.Priority = dto.Priority;
            request.Status = dto.Status;
            request.ResolutionNote = dto.ResolutionNote;
            request.EstimatedCost = dto.EstimatedCost;
            request.ActualCost = dto.ActualCost;
            request.AfterImageUrls = dto.AfterImageUrls;

            await _maintenanceRequestRepository.UpdateAsync(request);
            return NoContent();
        }

        [HttpPost("{id}/assign")]
        public async Task<ActionResult> Assign(int id, [FromBody] AssignMaintenanceRequest dto)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            var oldStatus = request.Status;
            request.Status = "Assigned";
            request.AssignedToUserId = dto.AssignedToUserId;
            request.AssignedToName = dto.AssignedToName;
            request.AssignedAt = DateTime.UtcNow;
            request.ExpectedCompletionDate = dto.ExpectedCompletionDate;
            request.EstimatedCost = dto.EstimatedCost;

            // Add status log
            var log = new MaintenanceStatusLog
            {
                MaintenanceRequestId = id,
                OldStatus = oldStatus,
                NewStatus = "Assigned",
                Note = $"Assigned to {dto.AssignedToName}",
                ChangedByUserId = dto.AssignedToUserId,
                ChangedByName = dto.AssignedToName,
                ChangedAt = DateTime.UtcNow
            };
            request.StatusLogs.Add(log);

            await _maintenanceRequestRepository.UpdateAsync(request);
            return NoContent();
        }

        [HttpPost("{id}/start")]
        public async Task<ActionResult> Start(int id)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            var oldStatus = request.Status;
            request.Status = "InProgress";
            request.StartedAt = DateTime.UtcNow;

            var log = new MaintenanceStatusLog
            {
                MaintenanceRequestId = id,
                OldStatus = oldStatus,
                NewStatus = "InProgress",
                Note = "Started working on the issue",
                ChangedByUserId = request.AssignedToUserId ?? 0,
                ChangedByName = request.AssignedToName ?? "System",
                ChangedAt = DateTime.UtcNow
            };
            request.StatusLogs.Add(log);

            await _maintenanceRequestRepository.UpdateAsync(request);
            return NoContent();
        }

        [HttpPost("{id}/resolve")]
        public async Task<ActionResult> Resolve(int id, [FromBody] ResolveMaintenanceRequest dto)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            var oldStatus = request.Status;
            request.Status = "Done";
            request.ResolvedAt = DateTime.UtcNow;
            request.ResolutionNote = dto.ResolutionNote;
            request.ActualCost = dto.ActualCost;
            request.AfterImageUrls = dto.AfterImageUrls;

            var log = new MaintenanceStatusLog
            {
                MaintenanceRequestId = id,
                OldStatus = oldStatus,
                NewStatus = "Done",
                Note = dto.ResolutionNote,
                ChangedByUserId = request.AssignedToUserId ?? 0,
                ChangedByName = request.AssignedToName ?? "System",
                ChangedAt = DateTime.UtcNow
            };
            request.StatusLogs.Add(log);

            await _maintenanceRequestRepository.UpdateAsync(request);
            return NoContent();
        }

        [HttpPost("{id}/reject")]
        public async Task<ActionResult> Reject(int id, [FromBody] RejectMaintenanceRequest dto)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            var oldStatus = request.Status;
            request.Status = "Rejected";
            request.RejectedReason = dto.RejectedReason;
            request.RejectedByUserId = dto.RejectedByUserId;

            var log = new MaintenanceStatusLog
            {
                MaintenanceRequestId = id,
                OldStatus = oldStatus,
                NewStatus = "Rejected",
                Note = dto.RejectedReason,
                ChangedByUserId = dto.RejectedByUserId,
                ChangedByName = "Admin",
                ChangedAt = DateTime.UtcNow
            };
            request.StatusLogs.Add(log);

            await _maintenanceRequestRepository.UpdateAsync(request);
            return NoContent();
        }

        [HttpPost("{id}/rate")]
        public async Task<ActionResult> Rate(int id, [FromBody] RateMaintenanceRequest dto)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            request.Rating = dto.Rating;
            request.Feedback = dto.Feedback;
            request.RatedAt = DateTime.UtcNow;

            await _maintenanceRequestRepository.UpdateAsync(request);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            await _maintenanceRequestRepository.DeleteAsync(request);
            return NoContent();
        }

        private static MaintenanceRequestDto MapToDto(MaintenanceRequest request)
        {
            return new MaintenanceRequestDto
            {
                Id = request.Id,
                RoomId = request.RoomId,
                RoomNumber = request.RoomNumber,
                BuildingName = request.BuildingName,
                RequestedByStudentId = request.RequestedByStudentId,
                RequestedByStudentName = request.RequestedByStudentName,
                Title = request.Title,
                Description = request.Description,
                Category = request.Category,
                Priority = request.Priority,
                ImageUrls = request.ImageUrls,
                Status = request.Status,
                AssignedToUserId = request.AssignedToUserId,
                AssignedToName = request.AssignedToName,
                AssignedAt = request.AssignedAt,
                StartedAt = request.StartedAt,
                ExpectedCompletionDate = request.ExpectedCompletionDate,
                ResolvedAt = request.ResolvedAt,
                ResolutionNote = request.ResolutionNote,
                EstimatedCost = request.EstimatedCost,
                ActualCost = request.ActualCost,
                AfterImageUrls = request.AfterImageUrls,
                Rating = request.Rating,
                Feedback = request.Feedback,
                RejectedReason = request.RejectedReason,
                CreatedAt = request.CreatedAt
            };
        }
    }
}
