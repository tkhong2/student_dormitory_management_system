using BillingMaintenanceService.Application.DTOs;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IPaymentService
    {
        Task<PaymentDto> ProcessPaymentAsync(int invoiceId, decimal amount, string method, string? note, int receivedByUserId, string receivedByName);
        Task<decimal> GetTotalDebtByStudentIdAsync(int studentId);
        Task<decimal> GetTotalDebtByContractIdAsync(int contractId);
    }
}
