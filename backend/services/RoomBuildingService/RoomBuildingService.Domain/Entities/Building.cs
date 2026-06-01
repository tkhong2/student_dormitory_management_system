namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Tòa nhà ký túc xá
/// </summary>
public class Building : BaseEntity
{
    public string Name { get; set; } = null!;               // Tên tòa: Tòa A - Nam
    public string Gender { get; set; } = null!;             // Male / Female / Mixed
    public int TotalFloors { get; set; }
    public int TotalRooms { get; set; }
    public string? Address { get; set; }
    public string? Description { get; set; }
    public string? ManagerName { get; set; }
    public string? ManagerPhone { get; set; }
    public string? ConstructionYear { get; set; }
    public string Status { get; set; } = "Active";          // Active / UnderMaintenance / Closed
    public bool HasElevator { get; set; } = false;
    public bool HasParking { get; set; } = false;
    public bool HasLaundry { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public string? ThumbnailUrl { get; set; }               // Ảnh đại diện tòa nhà

    // Navigation
    public ICollection<Floor> Floors { get; set; } = new List<Floor>();
    public ICollection<RoomType> RoomTypes { get; set; } = new List<RoomType>();
    public ICollection<BuildingAnnouncement> Announcements { get; set; } = new List<BuildingAnnouncement>();
}
