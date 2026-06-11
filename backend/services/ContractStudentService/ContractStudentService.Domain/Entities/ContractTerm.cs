namespace ContractStudentService.Domain.Entities;

/// <summary>
/// Điều khoản hợp đồng (Terms & Conditions)
/// </summary>
public class ContractTerm : BaseEntity
{
    public int ContractTemplateId { get; set; }                  // Thuộc mẫu hợp đồng nào
    public string Title { get; set; } = null!;                   // Tiêu đề điều khoản
    public string Content { get; set; } = null!;                 // Nội dung chi tiết
    public int OrderIndex { get; set; } = 0;                     // Thứ tự hiển thị
    public bool IsRequired { get; set; } = true;                 // Bắt buộc phải đọc/đồng ý
    public bool IsHighlighted { get; set; } = false;             // Làm nổi bật (quan trọng)
    public string? Icon { get; set; }                            // Icon hiển thị
    
    // Navigation
    public ContractTemplate ContractTemplate { get; set; } = null!;
}
