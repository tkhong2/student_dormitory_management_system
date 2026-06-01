namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Tầng trong tòa nhà
/// </summary>
public class Floor : BaseEntity
{
    public int BuildingId { get; set; }
    public int FloorNumber { get; set; }                   // 1, 2, 3...
    public string Label { get; set; } = null!;             // "Tầng 1", "Tầng trệt"
    public int TotalRooms { get; set; }
    public string? FloorPlanImageUrl { get; set; }         // Ảnh sơ đồ mặt bằng
    public bool IsActive { get; set; } = true;

    // Navigation
    public Building Building { get; set; } = null!;
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
}
