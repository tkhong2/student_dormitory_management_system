namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Sự kiện/Lịch sử bảo trì của tiện ích chung
/// </summary>
public class UtilityEvent : BaseEntity
{
    public int SharedUtilityId { get; set; }                 // FK → SharedUtility
    public string EventType { get; set; } = null!;           // Maintenance / Repair / Inspection / Replacement / Cleaning
    public string? Title { get; set; }                       // Tiêu đề sự kiện
    public string? Description { get; set; }                 // Mô tả chi tiết
    public DateTime EventDate { get; set; }                  // Ngày xảy ra sự kiện
    public DateTime? CompletedDate { get; set; }             // Ngày hoàn thành
    public string Status { get; set; } = "Scheduled";        // Scheduled / InProgress / Completed / Cancelled
    
    // Chi phí
    public decimal? EstimatedCost { get; set; }              // Chi phí ước tính
    public decimal? ActualCost { get; set; }                 // Chi phí thực tế
    
    // Người thực hiện
    public int? PerformedByUserId { get; set; }              // ref → BillingDB.Users
    public string? PerformedByName { get; set; }             // Tên người thực hiện
    public string? TechnicianCompany { get; set; }           // Công ty kỹ thuật viên
    public string? TechnicianContact { get; set; }           // Liên hệ kỹ thuật viên
    
    // Liên kết với phiếu báo trì (nếu có)
    public int? MaintenanceRequestId { get; set; }           // ref → BillingDB.MaintenanceRequests
    
    public string? Notes { get; set; }                       // Ghi chú bổ sung
    public string? ImageUrls { get; set; }                   // JSON array: ảnh trước/sau
    
    // Navigation
    public SharedUtility SharedUtility { get; set; } = null!;
}
