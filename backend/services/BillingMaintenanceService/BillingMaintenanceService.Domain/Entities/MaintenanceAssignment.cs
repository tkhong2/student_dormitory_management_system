namespace BillingMaintenanceService.Domain.Entities
{
    public class MaintenanceAssignment
    {
        public Guid Id { get; set; }
        public Guid MaintenanceRequestId { get; set; }
        public Guid StaffUserId { get; set; }
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        public string? Note { get; set; }

        public MaintenanceRequest? MaintenanceRequest { get; set; }
        public User? StaffUser { get; set; }
        public ICollection<MaintenanceLog> Logs { get; set; } = new List<MaintenanceLog>();
    }
}
