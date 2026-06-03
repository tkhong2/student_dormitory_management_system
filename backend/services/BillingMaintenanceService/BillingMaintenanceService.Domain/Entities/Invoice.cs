namespace BillingMaintenanceService.Domain.Entities;

/// <summary>
/// Phiếu thu tiền hàng tháng (hoặc phiếu thu tiền cọc)
/// Màn hình: NV > Danh sách phiếu thu, Thu tiền, Công nợ | SV > Phiếu thu của tôi
/// </summary>
public class Invoice : BaseEntity
{
    public string InvoiceCode { get; set; } = null!;            // PTT202401001

    // Loại phiếu thu
    public string InvoiceType { get; set; } = "Monthly";        // Monthly / Deposit / DepositRefund / Other

    // Ref IDs sang service khác (không FK cứng)
    public int ContractId { get; set; }                         // ref → ContractDB.Contracts
    public int StudentId { get; set; }                          // ref → ContractDB.Students
    public string StudentName { get; set; } = null!;            // Snapshot tên SV
    public string StudentCode { get; set; } = null!;            // Snapshot mã SV
    public int RoomId { get; set; }                             // ref → RoomDB.Rooms
    public string RoomNumber { get; set; } = null!;             // Snapshot số phòng
    public string BuildingName { get; set; } = null!;           // Snapshot tên tòa

    // Kỳ thanh toán
    public int BillingMonth { get; set; }
    public int BillingYear { get; set; }

    // Chi tiết các khoản tiền
    public decimal RentAmount { get; set; }                     // Tiền phòng
    public decimal ElectricityAmount { get; set; }              // Tiền điện
    public decimal WaterAmount { get; set; }                    // Tiền nước
    public decimal ServiceAmount { get; set; }                  // Phí dịch vụ (vệ sinh, wifi...)
    public decimal PreviousDebt { get; set; } = 0;              // Nợ kỳ trước chuyển sang
    public decimal Discount { get; set; } = 0;                  // Giảm giá (nếu có)
    public decimal PenaltyAmount { get; set; } = 0;             // Tiền phạt nộp trễ
    public decimal TotalAmount { get; set; }                    // Tổng phải thu
    public decimal PaidAmount { get; set; } = 0;                // Đã thanh toán
    public decimal DebtAmount { get; set; }                     // Còn nợ = Total - Paid

    // Trạng thái
    public string Status { get; set; } = "Unpaid";              // Unpaid / PartialPaid / Paid / Overdue / Cancelled
    public DateOnly DueDate { get; set; }                       // Hạn thanh toán
    public int OverdueDays { get; set; } = 0;                   // Số ngày quá hạn (tính tự động)

    // Nhắc nợ
    public int ReminderCount { get; set; } = 0;                 // Đã gửi nhắc bao nhiêu lần
    public DateTime? LastReminderSentAt { get; set; }           // Lần nhắc gần nhất

    public string? Notes { get; set; }
    public int CreatedByUserId { get; set; }

    // Navigation
    public User CreatedByUser { get; set; } = null!;
    public ICollection<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();
    public ICollection<Payment> Payments { get; set; } = new List<Payment>();
}

/// <summary>
/// Chi tiết từng khoản trong phiếu thu
/// Màn hình: NV/SV > Chi tiết phiếu thu (bảng kê từng khoản)
/// </summary>
public class InvoiceItem
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string ItemName { get; set; } = null!;               // Tiền phòng / Điện / Nước / Vệ sinh / Wifi
    public string? ItemDescription { get; set; }                // 120 kWh × 3.500đ = 420.000đ
    public decimal Quantity { get; set; } = 1;
    public string? Unit { get; set; }                           // kWh / m3 / tháng
    public decimal UnitPrice { get; set; }
    public decimal Amount { get; set; }                         // Quantity × UnitPrice
    public int SortOrder { get; set; } = 0;

    // Navigation
    public Invoice Invoice { get; set; } = null!;
}

/// <summary>
/// Lịch sử giao dịch thanh toán
/// Màn hình: NV > Ghi nhận thanh toán | SV > Lịch sử thanh toán
/// </summary>
public class Payment : BaseEntity
{
    public int InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = null!;                 // Cash / BankTransfer / Momo / VNPay / ZaloPay
    public string? TransactionCode { get; set; }                // Mã GD ngân hàng / ví điện tử
    public string? BankName { get; set; }                       // Tên ngân hàng
    public string? BankAccountNumber { get; set; }              // Số tài khoản nguồn
    public DateOnly PaymentDate { get; set; }
    public DateTime PaidAt { get; set; }
    public int ReceivedByUserId { get; set; }                   // NV thu tiền
    public string ReceivedByName { get; set; } = null!;         // Snapshot
    public string? ReceiptImageUrl { get; set; }                // Ảnh biên lai
    public string? Note { get; set; }

    // Navigation
    public Invoice Invoice { get; set; } = null!;
    public User ReceivedByUser { get; set; } = null!;
}
