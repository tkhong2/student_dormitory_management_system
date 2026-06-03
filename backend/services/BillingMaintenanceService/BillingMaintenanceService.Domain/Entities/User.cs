namespace BillingMaintenanceService.Domain.Entities;

/// <summary>
/// Tài khoản đăng nhập hệ thống
/// Màn hình: Trang đăng nhập, Admin > Quản lý tài khoản, Hồ sơ cá nhân, Đổi mật khẩu
/// </summary>
public class User : BaseEntity
{
    public string Username { get; set; } = null!;               // Mã SV hoặc username NV
    public string PasswordHash { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string Role { get; set; } = null!;                   // Admin / Staff / Student
    public string? AvatarUrl { get; set; }

    // Ref ID sang ContractService (nếu Role = Student)
    public int? StudentId { get; set; }                         // ref → ContractDB.Students
    public string? StudentCode { get; set; }                    // Snapshot mã SV

    // Trạng thái tài khoản
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public string? LastLoginIp { get; set; }
    public int FailedLoginAttempts { get; set; } = 0;
    public DateTime? LockedUntil { get; set; }                  // Khóa tạm thời do nhập sai nhiều lần

    // JWT Refresh token
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    // Đặt lại mật khẩu
    public string? PasswordResetToken { get; set; }
    public DateTime? PasswordResetExpiry { get; set; }

    // Cài đặt cá nhân
    public string? Preferences { get; set; }                    // JSON: { "theme": "dark", "language": "vi", "notifyEmail": true }

    // Tạo bởi (Admin tạo tài khoản NV)
    public int? CreatedByUserId { get; set; }

    // Navigation
    public ICollection<Invoice> CreatedInvoices { get; set; } = new List<Invoice>();
    public ICollection<Payment> ReceivedPayments { get; set; } = new List<Payment>();
    public ICollection<MaintenanceRequest> AssignedRequests { get; set; } = new List<MaintenanceRequest>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
