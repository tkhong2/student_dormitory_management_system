using BillingMaintenanceService.Domain.Enums;

namespace BillingMaintenanceService.Domain.Entities
{
    public class Debt
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public Guid StudentId { get; set; }
        public decimal Amount { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime DueDate { get; set; }
        public DebtStatus Status { get; set; } = DebtStatus.Open;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public Bill? Bill { get; set; }
    }
}
