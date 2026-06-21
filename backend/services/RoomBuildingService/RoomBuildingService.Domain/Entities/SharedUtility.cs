namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Tiện ích chung của tòa nhà (máy giặt, máy sấy, phòng gym, v.v.)
/// </summary>
public class SharedUtility : BaseEntity
{
    public int BuildingId { get; set; }                      // Tòa nhà sở hữu tiện ích
    public string Name { get; set; } = null!;                // Tên tiện ích: "Máy giặt tầng 1"
    public string Category { get; set; } = null!;            // WashingMachine / Dryer / Gym / StudyRoom / Kitchen / LaundryRoom
    public string? Brand { get; set; }                       // Tên thương hiệu: Samsung, LG, v.v.
    public string UtilityId { get; set; } = null!;           // ID tiện ích: WM-A-01, DRY-B-02
    public string Status { get; set; } = "Available";        // Available / InUse / Broken / UnderMaintenance / Retired
    public string? Location { get; set; }                    // Vị trí cụ thể: "Tầng 1, phòng giặt", "Tầng 2, gym"
    
    // Quản lý
    public string? ManagerName { get; set; }                 // Tên người quản lý
    public string? ManagerPhone { get; set; }                // Số điện thoại người quản lý
    public string? ManagerEmail { get; set; }                // Email người quản lý
    public string? SocialLinks { get; set; }                 // JSON: {"facebook": "...", "zalo": "..."}
    
    // Thông tin bổ sung
    public string? Description { get; set; }                 // Mô tả chi tiết
    public string? ImageUrl { get; set; }                    // Ảnh tiện ích
    public decimal? FeePerUse { get; set; }                  // Phí sử dụng mỗi lần
    public string? OperatingHours { get; set; }              // Giờ hoạt động: "6:00 - 22:00"
    public string? UsageInstructions { get; set; }           // Hướng dẫn sử dụng
    
    // Bảo trì
    public DateTime? PurchaseDate { get; set; }              // Ngày mua
    public DateTime? InstallationDate { get; set; }          // Ngày lắp đặt
    public DateTime? LastMaintenanceDate { get; set; }       // Lần bảo trì gần nhất
    public DateTime? NextMaintenanceDate { get; set; }       // Lần bảo trì tiếp theo
    public int? WarrantyMonths { get; set; }                 // Số tháng bảo hành
    public DateTime? WarrantyExpiry { get; set; }            // Ngày hết bảo hành
    
    // Thống kê
    public int TotalUsageCount { get; set; } = 0;            // Tổng số lần sử dụng
    public DateTime? LastUsedAt { get; set; }                // Lần sử dụng cuối
    
    // Navigation
    public Building Building { get; set; } = null!;
    public ICollection<UtilityEvent> Events { get; set; } = new List<UtilityEvent>();
    public ICollection<UtilityUsageLog> UsageLogs { get; set; } = new List<UtilityUsageLog>();
}
