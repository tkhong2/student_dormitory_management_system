using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByEmailAsync(string email);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(User user);
    }

    public interface IInvoiceRepository
    {
        Task<Invoice?> GetByIdAsync(int id);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<IEnumerable<Invoice>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<Invoice>> GetByContractIdAsync(int contractId);
        Task<Invoice?> GetByInvoiceCodeAsync(string invoiceCode);
        Task<Invoice> AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(Invoice invoice);
    }

    public interface IPaymentRepository
    {
        Task<Payment?> GetByIdAsync(int id);
        Task<IEnumerable<Payment>> GetAllAsync();
        Task<IEnumerable<Payment>> GetByInvoiceIdAsync(int invoiceId);
        Task<Payment> AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(Payment payment);
    }

    public interface IMaintenanceRequestRepository
    {
        Task<MaintenanceRequest?> GetByIdAsync(int id);
        Task<IEnumerable<MaintenanceRequest>> GetAllAsync();
        Task<IEnumerable<MaintenanceRequest>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<MaintenanceRequest>> GetByStudentIdAsync(int studentId);
        Task<MaintenanceRequest> AddAsync(MaintenanceRequest request);
        Task UpdateAsync(MaintenanceRequest request);
        Task DeleteAsync(MaintenanceRequest request);
    }
}