namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Kiểm tra chi tiết tình trạng tiện nghi trong phòng
/// Liên kết với RoomInspection và có thể tạo MaintenanceRequest
/// </summary>
public class RoomAmenityInspection : BaseEntity
{
    public int RoomInspectionId { get; set; }                   // ref → RoomInspection
    public int RoomId { get; set; }                             // ref → Room
    public string RoomNumber { get; set; } = null!;             // Snapshot
    
    // Tiện nghi được kiểm tra
    public int? AmenityId { get; set; }                         // ref → Amenity (nếu là tiện nghi chuẩn)
    public string AmenityName { get; set; } = null!;            // Tên tiện nghi (VD: Điều hòa, Giường, Tủ...)
    public string AmenityCategory { get; set; } = string.Empty; // Electric / Furniture / Sanitary / Other
    
    // Tình trạng
    public string Condition { get; set; } = "Good";             // Good / Fair / Poor / Broken / Missing
    public string? IssueDescription { get; set; }               // Mô tả chi tiết vấn đề
    public string? RecommendedAction { get; set; }              // Khuyến nghị (Repair / Replace / Monitor)
    
    // Hình ảnh
    public string? ImageUrls { get; set; }                      // JSON array ảnh chụp tình trạng
    
    // Liên kết với đơn bảo trì
    public int? MaintenanceRequestId { get; set; }              // ref → BillingDB.MaintenanceRequest (nếu đã tạo đơn)
    public bool NeedMaintenance { get; set; } = false;          // Có cần bảo trì không?
    public bool MaintenanceRequestCreated { get; set; } = false; // Đã tạo đơn bảo trì chưa?
    
    // Ưu tiên sửa chữa
    public string Priority { get; set; } = "Medium";            // Low / Medium / High / Urgent
    
    // Ghi chú
    public string? Notes { get; set; }
    
    // Navigation
    public RoomInspection RoomInspection { get; set; } = null!;
}
