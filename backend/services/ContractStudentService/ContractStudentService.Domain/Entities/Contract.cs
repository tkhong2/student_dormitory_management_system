namespace ContractStudentService.Domain.Entities;

/// <summary>
/// Hợp đồng thuê phòng
/// Màn hình: NV > Quản lý hợp đồng, SV > Hợp đồng của tôi
/// </summary>
public class Contract : BaseEntity
{
    public int StudentId { get; set; }
    public int ApplicationId { get; set; }

    // Ref IDs sang RoomService (không FK cứng)
    public int RoomId { get; set; }                         // ref → RoomService.Rooms
    public string RoomNumber { get; set; } = null!;         // Snapshot số phòng
    public int BuildingId { get; set; }                     // ref → RoomService.Buildings
    public string BuildingName { get; set; } = null!;       // Snapshot tên tòa
    public int RoomTypeId { get; set; }                     // ref → RoomService.RoomTypes
    public string RoomTypeName { get; set; } = null!;       // Snapshot tên loại phòng

    // Thông tin hợp đồng
    public string ContractCode { get; set; } = null!;       // HD2024001
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    // Giá tại thời điểm ký (snapshot — không thay đổi dù giá phòng tăng)
    public decimal MonthlyRent { get; set; }
    public decimal DepositAmount { get; set; }
    public bool IsDepositPaid { get; set; } = false;        // Đã thu tiền cọc chưa
    public DateTime? DepositPaidAt { get; set; }
    public decimal ElectricityRate { get; set; }
    public decimal WaterRate { get; set; }
    public int PaymentDueDay { get; set; } = 5;             // Thu tiền ngày mấy hàng tháng

    // Thông tin người ký
    public string? WitnessName { get; set; }                // Người làm chứng (quản lý KTX)
    public string? WitnessTitle { get; set; }               // Chức vụ người làm chứng
    public DateTime? SignedAt { get; set; }
    public string? SignedImageUrl { get; set; }             // Ảnh chữ ký / hợp đồng có dấu

    // Trạng thái
    public string Status { get; set; } = "Active";          // Pending / Active / Expired / Terminated / Transferred
    public DateTime? TerminatedAt { get; set; }
    public string? TerminatedReason { get; set; }
    public int? TerminatedByUserId { get; set; }            // ref → BillingDB.Users

    // Hoàn cọc khi kết thúc
    public decimal? DepositReturnedAmount { get; set; }
    public DateTime? DepositReturnedAt { get; set; }
    public string? DepositDeductionReason { get; set; }     // Lý do khấu trừ cọc (nếu có)

    public int CreatedByUserId { get; set; }                // ref → BillingDB.Users
    public string? Notes { get; set; }

    // Navigation
    public Student Student { get; set; } = null!;
    public RoomApplication Application { get; set; } = null!;
    public ICollection<ContractExtension> Extensions { get; set; } = new List<ContractExtension>();
    public ICollection<RoomTransfer> RoomTransfers { get; set; } = new List<RoomTransfer>();
}

/// <summary>
/// Gia hạn hợp đồng
/// Màn hình: NV > Chi tiết hợp đồng > Gia hạn, SV > Hợp đồng của tôi > Lịch sử gia hạn
/// </summary>
public class ContractExtension : BaseEntity
{
    public int ContractId { get; set; }
    public DateOnly OldEndDate { get; set; }
    public DateOnly NewEndDate { get; set; }
    public string? Reason { get; set; }
    public int ApprovedByUserId { get; set; }               // ref → BillingDB.Users
    public string ApprovedByName { get; set; } = null!;     // Snapshot
    public DateTime ApprovedAt { get; set; }

    // Navigation
    public Contract Contract { get; set; } = null!;
}

/// <summary>
/// Đơn xin chuyển phòng (SV đang ở muốn đổi phòng khác)
/// Màn hình: SV > Hợp đồng của tôi > Xin chuyển phòng, NV > Xử lý chuyển phòng
/// </summary>
public class RoomTransfer : BaseEntity
{
    public int ContractId { get; set; }
    public int StudentId { get; set; }

    // Phòng hiện tại
    public int CurrentRoomId { get; set; }                  // ref → RoomService.Rooms
    public string CurrentRoomNumber { get; set; } = null!;

    // Phòng muốn chuyển đến
    public int? RequestedRoomTypeId { get; set; }           // ref → RoomService.RoomTypes
    public string? RequestedRoomTypeName { get; set; }
    public int? RequestedBuildingId { get; set; }           // ref → RoomService.Buildings
    public string? RequestedBuildingName { get; set; }

    public string Reason { get; set; } = null!;             // Lý do muốn chuyển
    public string Status { get; set; } = "Pending";         // Pending / Approved / Rejected / Completed

    // Sau khi duyệt
    public int? NewRoomId { get; set; }                     // ref → RoomService.Rooms
    public string? NewRoomNumber { get; set; }
    public DateOnly? TransferDate { get; set; }             // Ngày chuyển phòng thực tế

    public int? ReviewedByUserId { get; set; }              // ref → BillingDB.Users
    public string? ReviewedByName { get; set; }
    public DateTime? ReviewedAt { get; set; }
    public string? RejectReason { get; set; }

    // Navigation
    public Contract Contract { get; set; } = null!;
}
