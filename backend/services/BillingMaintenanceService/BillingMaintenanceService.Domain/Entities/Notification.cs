namespace BillingMaintenanceService.Domain.Entities;

/// <summary>
/// Thông báo trong app gửi đến người dùng
/// Màn hình: Navbar badge số chưa đọc, Trang thông báo (tất cả role)
/// </summary>
public class Notification : BaseEntity
{
    public int UserId { get; set; }                             // Người nhận (có thể là UserId hoặc StudentId từ service khác)
    public string Title { get; set; } = null!;                  // Tiêu đề thông báo
    public string Body { get; set; } = null!;                   // Nội dung
    public string Type { get; set; } = null!;                   // ApplicationApproved / InvoiceCreated / MaintenanceDone / ContractExpiring / System
    public string? ActionUrl { get; set; }                      // Link điều hướng khi click (e.g. /invoices/123)
    public string? IconType { get; set; }                       // Icon hiển thị: success / warning / info / error
    public bool IsRead { get; set; } = false;
    public DateTime? ReadAt { get; set; }

    // Ref ID liên quan
    public int? RelatedEntityId { get; set; }                   // ID của entity liên quan (InvoiceId, RequestId...)
    public string? RelatedEntityType { get; set; }              // Invoice / MaintenanceRequest / Contract / Application

    // Navigation - made optional since UserId can be from different service (StudentId)
    public User? User { get; set; }
}

/// <summary>
/// Cài đặt hệ thống dạng key-value
/// Màn hình: Admin > Cài đặt hệ thống
/// </summary>
public class SystemSetting : BaseEntity
{
    public string Key { get; set; } = null!;                    // billing.payment_due_day / billing.penalty_rate / ktx.regulation_content
    public string Value { get; set; } = null!;                  // Giá trị dạng string (parse tùy theo key)
    public string DataType { get; set; } = "string";            // string / int / decimal / bool / json
    public string? Description { get; set; }                    // Mô tả cho Admin biết setting này dùng để làm gì
    public string? Group { get; set; }                          // Billing / Policy / Content / System
    public bool IsPublic { get; set; } = false;                 // Có hiển thị ra trang public không
    public int UpdatedByUserId { get; set; }                    // Admin cập nhật
    public string UpdatedByName { get; set; } = null!;
}

/// <summary>
/// Nhật ký hoạt động của hệ thống (audit trail)
/// Màn hình: Admin > Nhật ký hoạt động
/// </summary>
public class AuditLog
{
    public int Id { get; set; }
    public int UserId { get; set; }                             // Ai thực hiện
    public string UserName { get; set; } = null!;               // Snapshot
    public string UserRole { get; set; } = null!;               // Snapshot role
    public string Action { get; set; } = null!;                 // CREATE / UPDATE / DELETE / LOGIN / LOGOUT / APPROVE / REJECT
    public string EntityType { get; set; } = null!;             // Invoice / Contract / RoomApplication / MaintenanceRequest / User
    public int? EntityId { get; set; }                          // ID của entity bị tác động
    public string? OldValues { get; set; }                      // JSON giá trị cũ (trước khi thay đổi)
    public string? NewValues { get; set; }                      // JSON giá trị mới
    public string? Description { get; set; }                    // Mô tả hành động
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public User User { get; set; } = null!;
}

/// <summary>
/// Câu hỏi / liên hệ từ trang public (chưa đăng nhập)
/// Màn hình: Trang public > Liên hệ (form gửi) | Admin/NV > Xem & trả lời câu hỏi
/// </summary>
public class ContactInquiry : BaseEntity
{
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string Subject { get; set; } = null!;                // Chủ đề: Hỏi về phòng / Thủ tục đăng ký / Khác
    public string Message { get; set; } = null!;                // Nội dung câu hỏi
    public string Status { get; set; } = "New";                 // New / InProgress / Replied / Closed
    public string? ReplyContent { get; set; }                   // Nội dung phản hồi
    public int? RepliedByUserId { get; set; }                   // ref → BillingDB.Users
    public string? RepliedByName { get; set; }
    public DateTime? RepliedAt { get; set; }
    public string? IpAddress { get; set; }                      // IP người gửi (chống spam)
}
