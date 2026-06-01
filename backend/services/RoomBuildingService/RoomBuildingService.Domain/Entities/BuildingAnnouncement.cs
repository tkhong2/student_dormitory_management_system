namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Thông báo nội bộ của tòa nhà
/// (cúp điện, lịch vệ sinh, nội quy, sự kiện...)
/// Hiển thị ở Home SV và trang Thông báo
/// </summary>
public class BuildingAnnouncement : BaseEntity
{
    public int? BuildingId { get; set; }                     // null = thông báo toàn KTX
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Category { get; set; } = "General";      // General / Electric / Water / Maintenance / Event
    public string Priority { get; set; } = "Normal";       // Low / Normal / High / Urgent
    public bool IsPinned { get; set; } = false;            // Ghim lên đầu danh sách
    public DateTime? PublishedAt { get; set; }             // null = chưa đăng
    public DateTime? ExpiredAt { get; set; }               // Hết hạn hiển thị
    public int CreatedByUserId { get; set; }               // ref → BillingDB.Users
    public string CreatedByName { get; set; } = null!;     // Snapshot tên người đăng
    public string? ImageUrl { get; set; }

    // Navigation
    public Building? Building { get; set; }
}
