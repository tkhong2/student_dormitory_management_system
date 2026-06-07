namespace BillingMaintenanceService.Domain.Entities;

/// <summary>
/// Phân công tòa nhà cho nhân viên
/// Màn hình: Admin > Quản lý người dùng > Phân công tòa nhà
/// Mục đích: Giới hạn quyền duyệt đơn/hợp đồng của Staff theo tòa nhà được phân công
/// </summary>
public class BuildingAssignment : BaseEntity
{
    /// <summary>
    /// ID của nhân viên (User với Role = "Staff")
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID tòa nhà (ref → RoomService.Buildings)
    /// Không có FK cứng vì thuộc service khác
    /// </summary>
    public int BuildingId { get; set; }

    /// <summary>
    /// ID của Admin tạo phân công này
    /// </summary>
    public int CreatedByUserId { get; set; }

    // Navigation
    public User User { get; set; } = null!;
}
