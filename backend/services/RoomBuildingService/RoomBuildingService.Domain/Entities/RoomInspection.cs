namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Biên bản kiểm tra phòng
/// Admin/NV kiểm tra định kỳ hoặc khi có sự cố
/// Kết quả được duyệt bởi quản lý trước khi lưu chính thức
/// </summary>
public class RoomInspection : BaseEntity
{
    public int RoomId { get; set; }

    // Người kiểm tra
    public int InspectorUserId { get; set; }               // ref → BillingDB.Users
    public string InspectorName { get; set; } = null!;     // Snapshot
    public DateOnly InspectionDate { get; set; }
    public string InspectionType { get; set; } = "Routine"; // Routine / CheckIn / CheckOut / Incident

    // Kết quả kiểm tra
    public string OverallCondition { get; set; } = null!;  // Good / Fair / Poor
    public string ElectricalStatus { get; set; } = "OK";   // OK / Faulty / NeedsRepair
    public string PlumbingStatus { get; set; } = "OK";     // OK / Faulty / NeedsRepair
    public string FurnitureStatus { get; set; } = "OK";    // OK / Damaged / Missing
    public string WallStatus { get; set; } = "OK";         // OK / Dirty / Damaged
    public string FloorStatus { get; set; } = "OK";        // OK / Dirty / Damaged

    // Chỉ số đồng hồ
    public decimal? ElectricityReading { get; set; }       // Chỉ số điện kế (kWh)
    public decimal? WaterReading { get; set; }             // Chỉ số nước kế (m3)

    public string? Notes { get; set; }
    public string? ImageUrls { get; set; }                 // JSON array URL ảnh

    // Lên lịch kiểm tra tiếp theo
    public DateOnly? NextInspectionDate { get; set; }

    // Duyệt biên bản
    public bool IsApproved { get; set; } = false;
    public int? ApprovedByUserId { get; set; }             // ref → BillingDB.Users
    public string? ApprovedByName { get; set; }            // Snapshot
    public DateTime? ApprovedAt { get; set; }
    public string? ApprovalNote { get; set; }

    // Navigation
    public Room Room { get; set; } = null!;
}
