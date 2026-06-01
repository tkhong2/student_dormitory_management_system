namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Tiện nghi (Điều hòa, Tủ lạnh, TV, Máy giặt...)
/// Dùng chung, không gắn với tòa cụ thể
/// </summary>
public class Amenity : BaseEntity
{
    public string Name { get; set; } = null!;              // Điều hòa, Tủ lạnh, TV...
    public string Category { get; set; } = null!;          // Electric / Furniture / Sanitary / Other
    public string? IconUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<RoomTypeAmenity> RoomTypeAmenities { get; set; } = new List<RoomTypeAmenity>();
}
