namespace ContractStudentService.Domain.Entities;

/// <summary>
/// Hồ sơ sinh viên
/// Màn hình: NV > Danh sách SV, Hồ sơ chi tiết, SV > Hồ sơ cá nhân
/// </summary>
public class Student : BaseEntity
{
    // Thông tin học vấn
    public string StudentCode { get; set; } = null!;            // Mã SV: 2021001234
    public string Faculty { get; set; } = null!;                // Khoa/Viện
    public string Major { get; set; } = null!;                  // Ngành học
    public int AcademicYear { get; set; }                       // Năm học: 1, 2, 3, 4
    public string? ClassCode { get; set; }                      // Lớp: CNTT-K15

    // Thông tin cá nhân
    public string FullName { get; set; } = null!;
    public string Gender { get; set; } = null!;                 // Male / Female
    public DateOnly DateOfBirth { get; set; }
    public string? PlaceOfBirth { get; set; }
    public string? Ethnicity { get; set; }                      // Dân tộc
    public string? Religion { get; set; }                       // Tôn giáo
    public string? Nationality { get; set; } = "Việt Nam";
    public string? BloodType { get; set; }                      // Nhóm máu
    public string? HealthInsuranceNumber { get; set; }          // Số BHYT

    // Liên hệ
    public string Phone { get; set; } = null!;
    public string Email { get; set; } = null!;                  // Email trường/cá nhân

    // CCCD / CMND
    public string IdentityCard { get; set; } = null!;           // Số CCCD/CMND
    public DateOnly IdentityCardIssuedDate { get; set; }
    public string IdentityCardIssuedPlace { get; set; } = null!;

    // Địa chỉ
    public string PermanentAddress { get; set; } = null!;       // Địa chỉ thường trú
    public string? PermanentProvince { get; set; }              // Tỉnh/TP thường trú
    public string? TemporaryAddress { get; set; }               // Địa chỉ tạm trú

    // Liên hệ khẩn cấp
    public string EmergencyContactName { get; set; } = null!;
    public string EmergencyContactPhone { get; set; } = null!;
    public string EmergencyContactRelation { get; set; } = null!; // Bố / Mẹ / Anh / Chị
    public string? EmergencyContactAddress { get; set; }

    // Trạng thái
    public string? AvatarUrl { get; set; }                      // Ảnh chân dung
    public int ProfileCompletionPct { get; set; } = 0;          // % hồ sơ đã điền (0-100)
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }                          // Ghi chú của NV

    // Ref ID sang BillingDB (tài khoản đăng nhập của SV)
    public int? UserId { get; set; }                            // ref ID → BillingDB.Users

    // Navigation
    public ICollection<StudentDocument> Documents { get; set; } = new List<StudentDocument>();
    public ICollection<RoomApplication> Applications { get; set; } = new List<RoomApplication>();
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
