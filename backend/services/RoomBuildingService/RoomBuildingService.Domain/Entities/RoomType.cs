namespace RoomBuildingService.Domain.Entities;

public class RoomType : BaseEntity
{
    public int BuildingId { get; set; }
    public string Code { get; set; } = null!;              // DBL, QUAD, SINGLE, VIP...
    public string Name { get; set; } = null!;              // Phòng đôi, Phòng 4 người...
    public int Capacity { get; set; }                      // Sức chứa tối đa
    public decimal PricePerMonth { get; set; }             // Giá thuê/tháng
    public decimal DepositAmount { get; set; }             // Tiền đặt cọc
    public decimal ElectricityRate { get; set; }           // Giá điện (VNĐ/kWh)
    public decimal WaterRate { get; set; }                 // Giá nước (VNĐ/m3)
    public decimal Area { get; set; }                      // Diện tích m2
    public string BedType { get; set; } = "Single";        // Single / Bunk (giường tầng)
    public bool HasAirConditioner { get; set; } = false;
    public bool HasWaterHeater { get; set; } = false;
    public bool HasPrivateBathroom { get; set; } = false;
    public bool HasWindowView { get; set; } = false;
    public string? Description { get; set; }
    public string? ThumbnailUrl { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public Building Building { get; set; } = null!;
    public ICollection<Room> Rooms { get; set; } = new List<Room>();
    public ICollection<RoomTypeAmenity> RoomTypeAmenities { get; set; } = new List<RoomTypeAmenity>();
}
