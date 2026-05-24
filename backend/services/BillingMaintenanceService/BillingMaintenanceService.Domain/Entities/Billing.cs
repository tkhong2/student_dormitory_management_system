using BillingMaintenanceService.Domain.Enums;

namespace BillingMaintenanceService.Domain.Entities
{
    public class Bill
    {
        public Guid RoomId { get; set; }

        public Guid Id { get; set; }
        public Guid ContractId { get; set; } // Reference ID from ContractStudentService
        public Guid StudentId { get; set; } // Reference ID from ContractStudentService
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public BillStatus Status { get; set; } = BillStatus.Unpaid;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class Payment
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;

        // Navigation properties
        public Bill? Bill { get; set; }
    }

    public class MaintenanceRequest
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; } // Reference ID from RoomBuildingService
        public Guid StudentId { get; set; } // Reference ID from ContractStudentService
        public string Description { get; set; } = string.Empty;
        public MaintenanceStatus Status { get; set; } = MaintenanceStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; }
    }
}
