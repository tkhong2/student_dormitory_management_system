using BillingMaintenanceService.Domain.Enums;

namespace BillingMaintenanceService.Domain.Entities
{
    public class MaintenanceLog
    {
        public Guid Id { get; set; }
        public Guid MaintenanceRequestId { get; set; }
        public Guid? MaintenanceAssignmentId { get; set; }
        public Guid? CreatedByUserId { get; set; }
        public MaintenanceStatus Status { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public MaintenanceRequest? MaintenanceRequest { get; set; }
        public MaintenanceAssignment? MaintenanceAssignment { get; set; }
        public User? CreatedByUser { get; set; }
    }
}
