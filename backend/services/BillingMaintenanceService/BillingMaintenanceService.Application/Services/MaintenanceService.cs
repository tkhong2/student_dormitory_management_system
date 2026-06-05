using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.Application.Services
{
    public class MaintenanceService : IMaintenanceService
    {
        private readonly IMaintenanceRequestRepository _repository;

        public MaintenanceService(IMaintenanceRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<MaintenanceRequestDto> CreateRequestAsync(CreateMaintenanceRequestDto dto)
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
                Priority = dto.Priority ?? "Medium",
                ImageUrls = dto.ImageUrls,
                Status = "New",
                CreatedAt = DateTime.UtcNow
            };

            await _repository.AddAsync(request);
            
            // Add initial status log
            request.StatusLogs.Add(new MaintenanceStatusLog
            {
                OldStatus = "None",
                NewStatus = "New",
                Note = "Maintenance request created.",
                ChangedByUserId = dto.RequestedByStudentId, // assuming student is creating it
                ChangedByName = dto.RequestedByStudentName,
                ChangedAt = DateTime.UtcNow
            });
            await _repository.UpdateAsync(request);

            return MapToDto(request);
        }

        public async Task<IEnumerable<MaintenanceRequestDto>> GetAllRequestsAsync()
        {
            var requests = await _repository.GetAllAsync();
            return requests.Select(MapToDto);
        }

        public async Task<MaintenanceRequestDto?> GetRequestByIdAsync(int id)
        {
            var request = await _repository.GetByIdAsync(id);
            return request != null ? MapToDto(request) : null;
        }

        public async Task<IEnumerable<MaintenanceRequestDto>> GetRequestsByRoomIdAsync(int roomId)
        {
            var requests = await _repository.GetByRoomIdAsync(roomId);
            return requests.Select(MapToDto);
        }

        public async Task<IEnumerable<MaintenanceRequestDto>> GetRequestsByStudentIdAsync(int studentId)
        {
            var requests = await _repository.GetByStudentIdAsync(studentId);
            return requests.Select(MapToDto);
        }

        public async Task<MaintenanceRequestDto> UpdateStatusAsync(int id, UpdateMaintenanceStatusDto dto, int changedByUserId, string changedByName)
        {
            var request = await _repository.GetByIdAsync(id);
            if (request == null)
            {
                throw new Exception("Maintenance request not found");
            }

            var oldStatus = request.Status;
            request.Status = dto.NewStatus;
            request.UpdatedAt = DateTime.UtcNow;

            if (dto.NewStatus == "InProgress" && !request.StartedAt.HasValue)
            {
                request.StartedAt = DateTime.UtcNow;
            }
            else if (dto.NewStatus == "Done" || dto.NewStatus == "Completed")
            {
                request.ResolvedAt = DateTime.UtcNow;
            }

            request.StatusLogs.Add(new MaintenanceStatusLog
            {
                OldStatus = oldStatus,
                NewStatus = dto.NewStatus,
                Note = dto.Note,
                ChangedByUserId = changedByUserId,
                ChangedByName = changedByName,
                ChangedAt = DateTime.UtcNow
            });

            await _repository.UpdateAsync(request);
            return MapToDto(request);
        }

        private MaintenanceRequestDto MapToDto(MaintenanceRequest request)
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
