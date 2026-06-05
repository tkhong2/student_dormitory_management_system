using BillingMaintenanceService.Application.DTOs;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IMaintenanceService
    {
        Task<MaintenanceRequestDto> CreateRequestAsync(CreateMaintenanceRequestDto dto);
        Task<IEnumerable<MaintenanceRequestDto>> GetAllRequestsAsync();
        Task<IEnumerable<MaintenanceRequestDto>> GetRequestsByStudentIdAsync(int studentId);
        Task<IEnumerable<MaintenanceRequestDto>> GetRequestsByRoomIdAsync(int roomId);
        Task<MaintenanceRequestDto?> GetRequestByIdAsync(int id);
        Task<MaintenanceRequestDto> UpdateStatusAsync(int id, UpdateMaintenanceStatusDto dto, int changedByUserId, string changedByName);
    }
}
