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
    public class MaintenanceRequestsController : ControllerBase
    {
        private readonly IMaintenanceRequestRepository _maintenanceRequestRepository;
        private readonly IMaintenanceService _maintenanceService;

        public MaintenanceRequestsController(
            IMaintenanceRequestRepository maintenanceRequestRepository,
            IMaintenanceService maintenanceService)
        {
            _maintenanceRequestRepository = maintenanceRequestRepository;
            _maintenanceService = maintenanceService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetAll()
        {
            var requests = await _maintenanceRequestRepository.GetAllAsync();
            return Ok(requests.Select(MapToDto));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<MaintenanceRequestDto>> GetById(int id)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
            if (role == "Student")
            {
                var refIdStr = User.FindFirstValue("referenceId");
                if (!int.TryParse(refIdStr, out var refId) || refId != request.RequestedByStudentId)
                {
                    return Forbid();
                }
            }

            return Ok(MapToDto(request));
        }

        [HttpGet("room/{roomId:int}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetByRoomId(int roomId)
        {
            var requests = await _maintenanceRequestRepository.GetByRoomIdAsync(roomId);
            return Ok(requests.Select(MapToDto));
        }

        [HttpGet("student/{studentId:int}")]
        [Authorize(Roles = "Admin,Staff,Student")]
        public async Task<ActionResult<IEnumerable<MaintenanceRequestDto>>> GetByStudentId(int studentId)
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

            var requests = await _maintenanceRequestRepository.GetByStudentIdAsync(studentId);
            return Ok(requests.Select(MapToDto));
        }

        [HttpPost]
        public async Task<ActionResult<MaintenanceRequestDto>> Create([FromBody] MaintenanceRequestDto dto)
        {
            var request = new MaintenanceRequest();
            ApplyDto(request, dto);

            var role = User.FindFirstValue(ClaimTypes.Role) ?? User.FindFirstValue("role");
            if (role == "Student")
            {
                var refIdStr = User.FindFirstValue("referenceId");
                if (int.TryParse(refIdStr, out var refId))
                {
                    request.RequestedByStudentId = refId; // Force ownership
                }
            }

            request.CreatedAt = DateTime.UtcNow;
            request.UpdatedAt = DateTime.UtcNow;

            await _maintenanceRequestRepository.AddAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = request.Id }, MapToDto(request));
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult> Update(int id, [FromBody] MaintenanceRequestDto dto)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            ApplyDto(request, dto);
            request.UpdatedAt = DateTime.UtcNow;
            await _maintenanceRequestRepository.UpdateAsync(request);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int id)
        {
            var request = await _maintenanceRequestRepository.GetByIdAsync(id);
            if (request == null) return NotFound();

            await _maintenanceRequestRepository.DeleteAsync(request);
            return NoContent();
        }

        [HttpPut("{id:int}/status")]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<MaintenanceRequestDto>> UpdateStatus(int id, [FromBody] UpdateMaintenanceStatusDto dto)
        {
            try
            {
                var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub") ?? "0";
                var userName = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue("name") ?? "Unknown";
                int.TryParse(userIdStr, out int userId);

                var result = await _maintenanceService.UpdateStatusAsync(id, dto, userId, userName);
                return Ok(result);
            }
            catch (Exception ex)
            {
                if (ex.Message == "Maintenance request not found") return NotFound();
                return BadRequest(ex.Message);
            }
        }

        private static void ApplyDto(MaintenanceRequest request, MaintenanceRequestDto dto)
        {
            request.RoomId = dto.RoomId;
            request.RoomNumber = dto.RoomNumber;
            request.BuildingName = dto.BuildingName;
            request.RequestedByStudentId = dto.RequestedByStudentId;
            request.RequestedByStudentName = dto.RequestedByStudentName;
            request.Title = dto.Title;
            request.Description = dto.Description;
            request.Category = dto.Category;
            request.Priority = dto.Priority;
            request.ImageUrls = dto.ImageUrls;
            request.Status = string.IsNullOrWhiteSpace(dto.Status) ? "New" : dto.Status;
            request.AssignedToUserId = dto.AssignedToUserId;
            request.AssignedToName = dto.AssignedToName;
            request.AssignedAt = dto.AssignedAt;
            request.StartedAt = dto.StartedAt;
            request.ExpectedCompletionDate = dto.ExpectedCompletionDate;
            request.ResolvedAt = dto.ResolvedAt;
            request.ResolutionNote = dto.ResolutionNote;
            request.EstimatedCost = dto.EstimatedCost;
            request.ActualCost = dto.ActualCost;
            request.AfterImageUrls = dto.AfterImageUrls;
            request.IsRecurring = dto.IsRecurring;
            request.Rating = dto.Rating;
            request.Feedback = dto.Feedback;
            request.RatedAt = dto.RatedAt;
            request.RejectedReason = dto.RejectedReason;
            request.RejectedByUserId = dto.RejectedByUserId;
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
                IsRecurring = request.IsRecurring,
                Rating = request.Rating,
                Feedback = request.Feedback,
                RatedAt = request.RatedAt,
                RejectedReason = request.RejectedReason,
                RejectedByUserId = request.RejectedByUserId,
                CreatedAt = request.CreatedAt,
                UpdatedAt = request.UpdatedAt
            };
        }
    }
}