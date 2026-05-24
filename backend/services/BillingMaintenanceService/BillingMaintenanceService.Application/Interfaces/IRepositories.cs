using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IBillRepository
    {
        Task<Bill?> GetByIdAsync(Guid id);
        Task<IEnumerable<Bill>> GetAllAsync();
        Task AddAsync(Bill bill);
        Task UpdateAsync(Bill bill);
        Task DeleteAsync(Bill bill);
    }

    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetByBillIdAsync(Guid billId);
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(Payment payment);
    }

    public interface IMaintenanceRequestRepository
    {
        Task<MaintenanceRequest?> GetByIdAsync(Guid id);
        Task<IEnumerable<MaintenanceRequest>> GetAllAsync();
        Task<IEnumerable<MaintenanceRequest>> GetByRoomIdAsync(Guid roomId);
        Task AddAsync(MaintenanceRequest request);
        Task UpdateAsync(MaintenanceRequest request);
        Task DeleteAsync(MaintenanceRequest request);
    }
}