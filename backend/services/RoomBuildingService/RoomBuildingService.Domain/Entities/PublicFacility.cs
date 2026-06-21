namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Tiện ích chung (Phòng tập gym, Phòng học, Sân chơi, Giặt ủi...)
/// Màn hình: Admin > Quản lý tiện ích chung, Homepage > Danh sách tiện ích
/// </summary>
public class PublicFacility : BaseEntity
{
    public int? BuildingId { get; set; }                        // null = chung toàn KTX, có giá trị = thuộc tòa cụ thể
    public string? BuildingName { get; set; }                   // Snapshot tên tòa
    
    // Thông tin cơ bản
    public string Name { get; set; } = null!;                   // Phòng tập Gym, Phòng học chung, Sân bóng...
    public string Category { get; set; } = null!;               // Fitness / Study / Recreation / Laundry / Parking / Other
    public string BrandName { get; set; } = string.Empty;       // Thương hiệu (VD: Máy giặt Electrolux, Gym với thiết bị Life Fitness)
    public string FacilityId { get; set; } = string.Empty;      // Mã tiện ích (VD: GYM-01, WASH-A-01)
    public string Status { get; set; } = "Active";              // Active / UnderMaintenance / Closed / TemporarilyClosed
    
    // Mô tả
    public string? Description { get; set; }                    // Mô tả chi tiết
    public string? Location { get; set; }                       // Vị trí (VD: Tầng 1 Tòa A, Khu vực sân sau)
    public string? ImageUrl { get; set; }                       // Ảnh tiện ích
    
    // Thông tin người quản lý
    public string? ManagerName { get; set; }                    // Tên người quản lý
    public string? ManagerPhone { get; set; }                   // Số điện thoại
    public string? ManagerEmail { get; set; }                   // Email
    public string? SocialLinks { get; set; }                    // JSON: { "facebook": "...", "zalo": "..." }
    
    // Thời gian hoạt động
    public string? OperatingHours { get; set; }                 // JSON: { "monday": "6:00-22:00", "tuesday": "6:00-22:00", ... }
    public bool IsBookingRequired { get; set; } = false;        // Có cần đặt lịch không?
    public int? MaxCapacity { get; set; }                       // Sức chứa tối đa (nếu áp dụng)
    
    // Phí sử dụng
    public decimal? FeePerHour { get; set; }                    // Phí/giờ (null = miễn phí)
    public decimal? FeePerSession { get; set; }                 // Phí/lượt (VD: máy giặt 10k/lần)
    public string? FeeNote { get; set; }                        // Ghi chú về phí
    
    // Sự kiện đặc biệt
    public string? EventSchedule { get; set; }                  // JSON array: [{ "date": "2026-06-20", "event": "Bảo trì định kỳ", "from": "8:00", "to": "12:00" }]
    
    // Hiển thị
    public bool IsVisibleOnHomepage { get; set; } = true;       // Hiện ở trang chủ không?
    public int DisplayOrder { get; set; } = 0;                  // Thứ tự hiển thị
    public bool IsFeatured { get; set; } = false;               // Tiện ích nổi bật
    
    // Thống kê
    public int TotalUsageCount { get; set; } = 0;               // Tổng lượt sử dụng
    public DateTime? LastMaintenanceDate { get; set; }          // Lần bảo trì gần nhất
    public DateTime? NextMaintenanceDate { get; set; }          // Lần bảo trì kế tiếp
    
    // Navigation
    public ICollection<FacilityBooking> Bookings { get; set; } = new List<FacilityBooking>();
    public ICollection<FacilityReview> Reviews { get; set; } = new List<FacilityReview>();
}

/// <summary>
/// Đặt lịch sử dụng tiện ích chung
/// </summary>
public class FacilityBooking : BaseEntity
{
    public int FacilityId { get; set; }
    public int StudentId { get; set; }                          // ref → ContractDB.Students
    public string StudentName { get; set; } = null!;
    public string StudentPhone { get; set; } = null!;
    
    public DateOnly BookingDate { get; set; }
    public TimeOnly? StartTime { get; set; }                    // Giờ bắt đầu
    public TimeOnly? EndTime { get; set; }                      // Giờ kết thúc
    public string Status { get; set; } = "Pending";             // Pending / Confirmed / Cancelled / Completed
    
    public string? Purpose { get; set; }                        // Mục đích sử dụng
    public decimal? Fee { get; set; }                           // Phí phải trả
    public bool IsPaid { get; set; } = false;                   // Đã thanh toán chưa?
    
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }
    
    // Navigation
    public PublicFacility Facility { get; set; } = null!;
}

/// <summary>
/// Đánh giá tiện ích chung
/// </summary>
public class FacilityReview : BaseEntity
{
    public int FacilityId { get; set; }
    public int StudentId { get; set; }                          // ref → ContractDB.Students
    public string StudentName { get; set; } = null!;
    
    public int Rating { get; set; }                             // 1-5 sao
    public string? Comment { get; set; }
    public string? ImageUrl { get; set; }                       // Ảnh đánh giá (nếu có)
    
    public bool IsApproved { get; set; } = true;                // Admin duyệt hiển thị
    
    // Navigation
    public PublicFacility Facility { get; set; } = null!;
}
