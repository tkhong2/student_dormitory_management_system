namespace BillingMaintenanceService.Application.DTOs
{
    public class CreateMaintenanceRequestDto
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public int RequestedByStudentId { get; set; }
        public string RequestedByStudentName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string? ImageUrls { get; set; }
    }

    public class UpdateMaintenanceStatusDto
    {
        public string NewStatus { get; set; } = string.Empty; // "Assigned", "InProgress", "Done", "Cancelled"
        public string? Note { get; set; }
    }
}
