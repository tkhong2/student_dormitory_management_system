using BillingMaintenanceService.Domain.Enums;

namespace BillingMaintenanceService.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public UserRole Role { get; set; }
        public Guid? ReferenceId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
        public ICollection<MaintenanceAssignment> MaintenanceAssignments { get; set; } = new List<MaintenanceAssignment>();
        public ICollection<MaintenanceLog> MaintenanceLogs { get; set; } = new List<MaintenanceLog>();
    }
}
