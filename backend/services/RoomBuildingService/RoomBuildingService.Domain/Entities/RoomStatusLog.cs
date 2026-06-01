namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Log lịch sử thay đổi trạng thái phòng
/// Dùng để audit và hiển thị timeline
/// </summary>
public class RoomStatusLog
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public string OldStatus { get; set; } = null!;
    public string NewStatus { get; set; } = null!;
    public string? Reason { get; set; }
    public int ChangedByUserId { get; set; }               // ref → BillingDB.Users
    public string ChangedByName { get; set; } = null!;     // Snapshot
    public DateTime ChangedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Room Room { get; set; } = null!;
}
