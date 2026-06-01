namespace RoomBuildingService.Domain.Entities;

public class Room : BaseEntity
{
    public int FloorId { get; set; }
    public int RoomTypeId { get; set; }
    public string RoomNumber { get; set; } = null!;        // A101, B203...
    public string Status { get; set; } = "Available";      // Available / Full / Maintenance / Reserved / Closed
    public int CurrentOccupants { get; set; } = 0;         // Số SV đang ở thực tế
    public int MaxOccupants { get; set; }                  // Copy từ RoomType.Capacity khi tạo phòng
    public string? Orientation { get; set; }               // Đông / Tây / Nam / Bắc
    public string? Notes { get; set; }

    // Khóa phòng (admin tạm thời không cho đăng ký)
    public bool IsLocked { get; set; } = false;
    public string? LockReason { get; set; }
    public int? LockedByUserId { get; set; }               // ref → BillingDB.Users

    // Check-in bằng QR
    public string? QRCode { get; set; }                    // UUID hoặc mã ngắn để gen QR

    // Kiểm tra
    public DateTime? LastInspectedAt { get; set; }
    public DateTime? AvailableFrom { get; set; }           // Ngày có thể nhận SV mới (sau sửa chữa)

    // Navigation
    public Floor Floor { get; set; } = null!;
    public RoomType RoomType { get; set; } = null!;
    public ICollection<RoomImage> Images { get; set; } = new List<RoomImage>();
    public ICollection<RoomStatusLog> StatusLogs { get; set; } = new List<RoomStatusLog>();
    public ICollection<RoomInspection> Inspections { get; set; } = new List<RoomInspection>();
    public ICollection<RoomReservation> Reservations { get; set; } = new List<RoomReservation>();
}
