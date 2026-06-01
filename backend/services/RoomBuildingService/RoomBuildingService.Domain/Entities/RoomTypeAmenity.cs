namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Bảng nối RoomType ↔ Amenity
/// Ghi số lượng và ghi chú tiện nghi trong loại phòng
/// </summary>
public class RoomTypeAmenity
{
    public int Id { get; set; }
    public int RoomTypeId { get; set; }
    public int AmenityId { get; set; }
    public int Quantity { get; set; } = 1;
    public string? Note { get; set; }                      // "Còn hoạt động", "Cần thay"...

    // Navigation
    public RoomType RoomType { get; set; } = null!;
    public Amenity Amenity { get; set; } = null!;
}
