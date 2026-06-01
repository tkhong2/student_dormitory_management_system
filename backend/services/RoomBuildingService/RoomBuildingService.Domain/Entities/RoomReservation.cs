namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Đặt chỗ tạm thời khi admin đang xử lý đơn đăng ký
/// Tránh race condition: 2 admin cùng duyệt 1 phòng cho 2 SV khác nhau
/// Tự hết hạn sau ExpiresAt nếu không được xác nhận
/// </summary>
public class RoomReservation : BaseEntity
{
    public int RoomId { get; set; }

    // ref sang ContractDB — không FK cứng
    public int ApplicationId { get; set; }                 // ref → ContractDB.RoomApplications
    public int StudentId { get; set; }                     // ref → ContractDB.Students
    public string StudentName { get; set; } = null!;       // Snapshot

    public string Status { get; set; } = "Holding";        // Holding / Confirmed / Released / Expired
    public DateTime ExpiresAt { get; set; }                // Thường = CreatedAt + 24h
    public string? Note { get; set; }
    public int CreatedByUserId { get; set; }               // Admin tạo đặt chỗ
    public string CreatedByName { get; set; } = null!;     // Snapshot
    public DateTime? ReleasedAt { get; set; }              // Khi hủy hoặc xác nhận xong
    public string? ReleaseReason { get; set; }             // "Confirmed" / "AdminCancelled" / "Expired"

    // Navigation
    public Room Room { get; set; } = null!;
}
