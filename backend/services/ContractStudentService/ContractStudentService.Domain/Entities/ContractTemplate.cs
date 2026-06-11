namespace ContractStudentService.Domain.Entities;

/// <summary>
/// Mẫu hợp đồng với các điều khoản
/// </summary>
public class ContractTemplate : BaseEntity
{
    public string Name { get; set; } = null!;                    // Tên mẫu: "Hợp đồng KTX tiêu chuẩn", "Hợp đồng VIP"
    public string Code { get; set; } = null!;                    // Mã: STANDARD, VIP, SHORT_TERM
    public string? Description { get; set; }                     // Mô tả mẫu
    public bool IsDefault { get; set; } = false;                 // Mẫu mặc định
    public bool IsActive { get; set; } = true;                   // Đang sử dụng
    public int? MinDurationMonths { get; set; }                  // Thời hạn tối thiểu (tháng)
    public int? MaxDurationMonths { get; set; }                  // Thời hạn tối đa (tháng)
    public string? TemplateContent { get; set; }                 // Nội dung hợp đồng đầy đủ (HTML/Markdown)
    
    // Navigation
    public ICollection<ContractTerm> Terms { get; set; } = new List<ContractTerm>();
    public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
