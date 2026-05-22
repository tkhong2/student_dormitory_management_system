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
        public UserRole Role { get; set; }
        public Guid? ReferenceId { get; set; } // Can be StudentId if role is Student
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
