using BillingMaintenanceService.Domain.Enums;

namespace BillingMaintenanceService.Domain.Entities
{
    public class Debt : BaseEntity
    {
        public int InvoiceId { get; set; }
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateOnly DueDate { get; set; }
        public DebtStatus Status { get; set; } = DebtStatus.Open;

        public Invoice? Invoice { get; set; }
    }
}
