namespace BillingMaintenanceService.Domain.Entities;

/// <summary>
/// Yêu cầu sửa chữa / báo hỏng của sinh viên
/// Màn hình: SV > Gửi sự cố, Theo dõi sự cố | NV > Xử lý sự cố | Admin > Báo cáo bảo trì
/// </summary>
public class MaintenanceRequest : BaseEntity
{
    // Ref IDs sang service khác
    public int RoomId { get; set; }                             // ref → RoomDB.Rooms
    public string RoomNumber { get; set; } = null!;             // Snapshot số phòng
    public string BuildingName { get; set; } = null!;           // Snapshot tên tòa
    public int RequestedByStudentId { get; set; }               // ref → ContractDB.Students
    public string RequestedByStudentName { get; set; } = null!; // Snapshot tên SV

    // Nội dung yêu cầu
    public string Title { get; set; } = null!;                  // Tiêu đề ngắn gọn
    public string Description { get; set; } = null!;            // Mô tả chi tiết
    public string Category { get; set; } = null!;               // Electric / Plumbing / Furniture / Network / Structure / Other
    public string Priority { get; set; } = "Medium";            // Low / Medium / High / Urgent
    public string? ImageUrls { get; set; }                      // JSON array ảnh đính kèm (trước khi sửa)

    // Xử lý
    public string Status { get; set; } = "New";                 // New / Assigned / InProgress / Done / Cancelled / Rejected
    public int? AssignedToUserId { get; set; }                  // ref → BillingDB.Users (kỹ thuật viên)
    public string? AssignedToName { get; set; }                 // Snapshot tên kỹ thuật viên
    public DateTime? AssignedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateOnly? ExpectedCompletionDate { get; set; }       // Ngày dự kiến hoàn thành
    public DateTime? ResolvedAt { get; set; }
    public string? ResolutionNote { get; set; }                 // Ghi chú sau khi xử lý
    public decimal? EstimatedCost { get; set; }                 // Chi phí dự kiến
    public decimal? ActualCost { get; set; }                    // Chi phí thực tế
    public string? AfterImageUrls { get; set; }                 // JSON array ảnh sau khi sửa
    public bool IsRecurring { get; set; } = false;              // Sự cố tái diễn nhiều lần

    // Đánh giá của SV (sau khi Done)
    public int? Rating { get; set; }                            // 1-5 sao
    public string? Feedback { get; set; }                       // Nhận xét của SV
    public DateTime? RatedAt { get; set; }

    // Từ chối (nếu không hợp lệ)
    public string? RejectedReason { get; set; }
    public int? RejectedByUserId { get; set; }

    // Navigation
    public User? AssignedToUser { get; set; }
    public ICollection<MaintenanceStatusLog> StatusLogs { get; set; } = new List<MaintenanceStatusLog>();
}

/// <summary>
/// Lịch sử thay đổi trạng thái yêu cầu sửa chữa
/// Màn hình: SV/NV > Timeline xử lý sự cố
/// </summary>
public class MaintenanceStatusLog
{
    public int Id { get; set; }
    public int MaintenanceRequestId { get; set; }
    public string OldStatus { get; set; } = null!;
    public string NewStatus { get; set; } = null!;
    public string? Note { get; set; }
    public int ChangedByUserId { get; set; }
    public string ChangedByName { get; set; } = null!;
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public MaintenanceRequest MaintenanceRequest { get; set; } = null!;
}
