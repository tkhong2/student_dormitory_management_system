namespace BillingMaintenanceService.Application.DTOs
{
    public class JwtTokenResultDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
    }

    public class BillDto
    {
        public Guid Id { get; set; }
        public string BillCode { get; set; } = string.Empty;
        public Guid ContractId { get; set; }
        public Guid StudentId { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public DateOnly PaymentDate { get; set; }
        public DateTime PaidAt { get; set; }
        public int ReceivedByUserId { get; set; }
        public string ReceivedByName { get; set; } = string.Empty;
        public string? ReceiptImageUrl { get; set; }
        public string? Note { get; set; }
    }

    public class MaintenanceRequestDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public int RequestedByStudentId { get; set; }
        public string RequestedByStudentName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = "Medium";
        public string? ImageUrls { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? AssignedToUserId { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? StartedAt { get; set; }
        public DateOnly? ExpectedCompletionDate { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? ResolutionNote { get; set; }
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public string? AfterImageUrls { get; set; }
        public bool IsRecurring { get; set; }
        public int? Rating { get; set; }
        public string? Feedback { get; set; }
        public DateTime? RatedAt { get; set; }
        public string? RejectedReason { get; set; }
        public int? RejectedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class InvoiceItemDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public string? ItemDescription { get; set; }
        public decimal Quantity { get; set; }
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Amount { get; set; }
        public int SortOrder { get; set; }
    }

    public class CreateInvoiceItemDto
    {
        public string ItemName { get; set; } = string.Empty;
        public string? ItemDescription { get; set; }
        public decimal Quantity { get; set; } = 1;
        public string? Unit { get; set; }
        public decimal UnitPrice { get; set; }
        public int SortOrder { get; set; }
    }

    public class InvoiceDto
    {
        public int Id { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
        public string InvoiceType { get; set; } = string.Empty;
        public int ContractId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public decimal RentAmount { get; set; }
        public decimal ElectricityAmount { get; set; }
        public decimal WaterAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal PreviousDebt { get; set; }
        public decimal Discount { get; set; }
        public decimal PenaltyAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DebtAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateOnly DueDate { get; set; }
        public int OverdueDays { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<InvoiceItemDto> Items { get; set; } = new();
    }

    public class CreateInvoiceDto
    {
        public string InvoiceCode { get; set; } = string.Empty;
        public string InvoiceType { get; set; } = "Monthly";
        public int ContractId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentCode { get; set; } = string.Empty;
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string BuildingName { get; set; } = string.Empty;
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public decimal RentAmount { get; set; }
        public decimal ElectricityAmount { get; set; }
        public decimal WaterAmount { get; set; }
        public decimal ServiceAmount { get; set; }
        public decimal PreviousDebt { get; set; }
        public decimal Discount { get; set; }
        public DateOnly DueDate { get; set; }
        public int CreatedByUserId { get; set; }
        public string? Notes { get; set; }
        public List<CreateInvoiceItemDto> Items { get; set; } = new();
    }

    public class ContractDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int BuildingId { get; set; }
        public string BuildingName { get; set; } = string.Empty;
        public int RoomTypeId { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public string ContractCode { get; set; } = string.Empty;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal ElectricityRate { get; set; }
        public decimal WaterRate { get; set; }
        public int PaymentDueDay { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class GenerateMonthlyInvoiceRequestDto
    {
        public int ContractId { get; set; }
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public int CreatedByUserId { get; set; }
    }

    public class GenerateMonthlyInvoicesRequestDto
    {
        public int BillingMonth { get; set; }
        public int BillingYear { get; set; }
        public int CreatedByUserId { get; set; }
    }

    public class LoginRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginRequest : LoginRequestDto
    {
    }

    public class RegisterRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        // Optional. In Development, the first registered account may set this to Admin/Staff/Student.
        // Otherwise it will default to Student.
        public string? Role { get; set; }
        public Guid? ReferenceId { get; set; }
    }

    public class RegisterRequest : RegisterRequestDto
    {
    }

    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserResponse? User { get; set; }
    }

    public class LoginResponse : AuthResponseDto
    {
    }

    public class CreateUserRequestDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateUserDto : CreateUserRequestDto
    {
    }

    public class UpdateUserRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public bool? IsActive { get; set; }
    }

    public class SetActiveRequestDto
    {
        public bool IsActive { get; set; }
    }

    public class ResetPasswordRequestDto
    {
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangeMyPasswordRequestDto
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class UserDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Role { get; set; } = string.Empty;
        public Guid? ReferenceId { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UserResponse : UserDto
    {
    }
}
