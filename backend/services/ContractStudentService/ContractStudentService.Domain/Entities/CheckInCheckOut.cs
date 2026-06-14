namespace ContractStudentService.Domain.Entities;

/// <summary>
/// Quy trình Check-in (nhận phòng)
/// Màn hình: Staff > Check-in/Check-out
/// </summary>
public class CheckIn : BaseEntity
{
    public int ContractId { get; set; }
    public int StudentId { get; set; }
    public int RoomId { get; set; }                         // ref → RoomService.Rooms
    public string RoomNumber { get; set; } = null!;         // Snapshot
    public string BuildingName { get; set; } = null!;       // Snapshot

    // Thông tin check-in
    public DateTime CheckInDate { get; set; }
    public int CheckedInByUserId { get; set; }              // ref → BillingDB.Users
    public string CheckedInByName { get; set; } = null!;    // Snapshot

    // Xác minh thông tin
    public string? IdCardImageUrls { get; set; }            // JSON array URLs ảnh CMND/CCCD
    public bool IsDepositPaid { get; set; } = false;
    public decimal DepositAmount { get; set; }
    public DateTime? DepositPaidAt { get; set; }

    // Kiểm tra phòng ban đầu
    public string? RoomConditionChecklist { get; set; }     // JSON array: walls, doors, electric, water, furniture, clean
    public string? RoomImageUrls { get; set; }              // JSON array URLs ảnh phòng ban đầu
    public string RoomCondition { get; set; } = "Good";     // Good / Fair / Poor
    public string? Notes { get; set; }

    // Chỉ số đồng hồ ban đầu
    public decimal? InitialElectricityReading { get; set; }
    public decimal? InitialWaterReading { get; set; }

    // Key/Access card
    public string? KeysProvided { get; set; }               // Mô tả chìa khóa/thẻ đã giao
    public int? KeyCount { get; set; }

    public string Status { get; set; } = "Completed";       // Completed / Cancelled

    // Navigation
    public Contract Contract { get; set; } = null!;
    public Student Student { get; set; } = null!;
}

/// <summary>
/// Quy trình Check-out (trả phòng)
/// Màn hình: Staff > Check-in/Check-out
/// </summary>
public class CheckOut : BaseEntity
{
    public int ContractId { get; set; }
    public int StudentId { get; set; }
    public int RoomId { get; set; }                         // ref → RoomService.Rooms
    public string RoomNumber { get; set; } = null!;         // Snapshot
    public string BuildingName { get; set; } = null!;       // Snapshot

    // Thông tin check-out
    public DateTime CheckOutDate { get; set; }
    public int CheckedOutByUserId { get; set; }             // ref → BillingDB.Users
    public string CheckedOutByName { get; set; } = null!;   // Snapshot

    // Kiểm tra phòng khi trả
    public string? CurrentRoomImageUrls { get; set; }       // JSON array URLs ảnh phòng hiện tại
    public string RoomCondition { get; set; } = "Good";     // Good / Minor / Major (damage)
    public string? DamageDescription { get; set; }          // Mô tả hư hỏng (nếu có)

    // Tính toán chi phí
    public decimal DepositAmount { get; set; }              // Tiền cọc ban đầu
    public decimal CompensationCost { get; set; } = 0;      // Chi phí bồi thường hư hỏng
    public decimal RefundAmount { get; set; }               // Số tiền hoàn trả = Deposit - Compensation
    public string? CompensationDetails { get; set; }        // Chi tiết bồi thường

    // Chỉ số đồng hồ cuối
    public decimal? FinalElectricityReading { get; set; }
    public decimal? FinalWaterReading { get; set; }

    // Hoàn trả
    public bool IsKeyReturned { get; set; } = false;        // Đã thu hồi chìa khóa/thẻ
    public bool IsDepositRefunded { get; set; } = false;    // Đã hoàn tiền cọc
    public DateTime? DepositRefundedAt { get; set; }
    public string? RefundMethod { get; set; }               // Cash / BankTransfer
    public string? RefundReference { get; set; }            // Số phiếu thu/chuyển khoản

    public string? Notes { get; set; }
    public string Status { get; set; } = "Completed";       // Completed / Cancelled

    // Navigation
    public Contract Contract { get; set; } = null!;
    public Student Student { get; set; } = null!;
}
