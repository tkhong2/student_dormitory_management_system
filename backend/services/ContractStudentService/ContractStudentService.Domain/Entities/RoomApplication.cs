namespace ContractStudentService.Domain.Entities;

/// <summary>
/// Đơn đăng ký phòng của sinh viên
/// Màn hình: SV > Form đăng ký phòng, Theo dõi đơn | NV > Duyệt đơn
/// </summary>
public class RoomApplication : BaseEntity
{
    public int StudentId { get; set; }

    // Thông tin mong muốn (ref ID sang RoomService — không FK cứng)
    public int PreferredBuildingId { get; set; }
    public string PreferredBuildingName { get; set; } = null!;  // Snapshot tên tòa
    public int PreferredRoomTypeId { get; set; }
    public string PreferredRoomTypeName { get; set; } = null!;  // Snapshot tên loại phòng
    public decimal PreferredRoomPrice { get; set; }             // Snapshot giá lúc đăng ký

    // Thời gian ở
    public DateOnly RequestedStartDate { get; set; }
    public DateOnly RequestedEndDate { get; set; }

    // Yêu cầu đặc biệt
    public string? SpecialRequirements { get; set; }            // Yêu cầu về phòng, bạn cùng phòng...
    public string? Note { get; set; }                           // Ghi chú thêm
    public bool IsLocalStudent { get; set; } = false;           // SV nội tỉnh
    public int Priority { get; set; } = 0;                      // Độ ưu tiên (SV năm 1, SV ngoại tỉnh...)

    // Giấy tờ đính kèm đơn
    public string? AttachedDocumentUrls { get; set; }           // JSON array URL giấy tờ kèm đơn

    // Trạng thái duyệt
    public string Status { get; set; } = "Pending";             // Pending / UnderReview / Approved / Rejected / Cancelled
    public int? ReviewedByUserId { get; set; }                  // ref ID → BillingDB.Users
    public string? ReviewedByName { get; set; }                 // Snapshot tên NV duyệt
    public DateTime? ReviewedAt { get; set; }
    public string? RejectReason { get; set; }                   // Lý do từ chối

    // Phòng được xếp (sau khi duyệt)
    public int? AssignedRoomId { get; set; }                    // ref ID → RoomService.Rooms
    public string? AssignedRoomNumber { get; set; }             // Snapshot số phòng được xếp
    public string? AssignedBuildingName { get; set; }           // Snapshot tên tòa được xếp

    // Hủy đơn
    public DateTime? CancelledAt { get; set; }
    public string? CancelledReason { get; set; }
    public bool CancelledBySelf { get; set; } = false;          // SV tự hủy hay NV hủy

    // Navigation
    public Student Student { get; set; } = null!;
    public Contract? Contract { get; set; }
}
