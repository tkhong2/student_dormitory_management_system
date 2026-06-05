using BillingMaintenanceService.Application.DTOs;

namespace BillingMaintenanceService.Application.Interfaces
{
    public interface IInvoiceService
    {
        Task<InvoiceDto> GenerateMonthlyInvoiceAsync(int contractId, int billingMonth, int billingYear, int createdByUserId);
        Task<IEnumerable<InvoiceDto>> GenerateMonthlyInvoicesForMonthAsync(int billingMonth, int billingYear, int createdByUserId);
        Task<int> ProcessOverdueInvoicesAsync(int reminderIntervalDays);
    }
}
