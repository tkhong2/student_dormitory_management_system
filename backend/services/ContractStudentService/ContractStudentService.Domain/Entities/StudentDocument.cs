namespace ContractStudentService.Domain.Entities;

/// <summary>
/// Giấy tờ đính kèm hồ sơ sinh viên
/// Màn hình: SV > Hồ sơ cá nhân (upload), NV > Duyệt đơn (xem giấy tờ)
/// </summary>
public class StudentDocument : BaseEntity
{
    public int StudentId { get; set; }
    public string DocumentType { get; set; } = null!;       // IdentityCard / StudentCard / HouseholdBook / HealthInsurance / Other
    public string DocumentName { get; set; } = null!;       // Tên tài liệu hiển thị
    public string FileUrl { get; set; } = null!;            // URL file (ảnh/PDF)
    public string FileType { get; set; } = null!;           // image/jpeg, application/pdf
    public long FileSizeBytes { get; set; }
    public bool IsVerified { get; set; } = false;           // NV đã xác minh chưa
    public int? VerifiedByUserId { get; set; }              // ref ID → BillingDB.Users
    public DateTime? VerifiedAt { get; set; }
    public string? Notes { get; set; }

    // Navigation
    public Student Student { get; set; } = null!;
}
