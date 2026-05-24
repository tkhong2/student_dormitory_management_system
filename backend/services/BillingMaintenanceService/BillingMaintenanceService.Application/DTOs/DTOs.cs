namespace BillingMaintenanceService.Application.DTOs
{
    public class BillDto
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public Guid StudentId { get; set; }
        public Guid RoomId { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid BillId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
    }

    public class MaintenanceRequestDto
    {
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid StudentId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string? Note { get; set; }
    }
}