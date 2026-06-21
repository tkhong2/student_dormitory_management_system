namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Lịch sử sử dụng tiện ích chung (tracking)
/// </summary>
public class UtilityUsageLog : BaseEntity
{
    public int SharedUtilityId { get; set; }                 // FK → SharedUtility
    public int? StudentId { get; set; }                      // ref → ContractDB.Students
    public string? StudentName { get; set; }                 // Tên sinh viên
    public string? RoomNumber { get; set; }                  // Phòng của SV
    public DateTime StartTime { get; set; }                  // Thời gian bắt đầu sử dụng
    public DateTime? EndTime { get; set; }                   // Thời gian kết thúc
    public decimal? FeeCharged { get; set; }                 // Phí đã tính
    public bool IsPaid { get; set; } = false;                // Đã thanh toán chưa
    public int? InvoiceId { get; set; }                      // ref → BillingDB.Invoices (nếu đã lập phiếu thu)
    public string? Notes { get; set; }                       // Ghi chú
    
    // Navigation
    public SharedUtility SharedUtility { get; set; } = null!;
}
