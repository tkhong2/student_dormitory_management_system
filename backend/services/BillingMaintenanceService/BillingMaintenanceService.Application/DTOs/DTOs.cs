namespace BillingMaintenanceService.Application.DTOs
{
    // ===== User DTOs =====
    public class UserDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public int? StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? Faculty { get; set; }
        public string? ClassCode { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        
        // Social Media Links
        public string? FacebookUrl { get; set; }
        public string? ZaloPhone { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUserDto
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? StudentId { get; set; }
        public string? StudentCode { get; set; }
        public string? Faculty { get; set; }
        public string? ClassCode { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? Address { get; set; }
        
        // Social Media Links
        public string? FacebookUrl { get; set; }
        public string? ZaloPhone { get; set; }
        public string? InstagramUrl { get; set; }
        public string? LinkedInUrl { get; set; }
    }

    // ===== Invoice DTOs =====
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

    public class GenerateInvoiceFromContractDto
    {
        public string InvoiceCode { get; set; } = string.Empty;
        public string? InvoiceType { get; set; } = "Monthly";
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

    // ===== InvoiceItem DTOs =====
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
        public int SortOrder { get; set; } = 0;
    }

    // ===== Payment DTOs =====
    public class PaymentDto
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public string InvoiceCode { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
        public string? BankName { get; set; }
        public DateOnly PaymentDate { get; set; }
        public DateTime PaidAt { get; set; }
        public int ReceivedByUserId { get; set; }
        public string ReceivedByName { get; set; } = string.Empty;
        public string? ReceiptImageUrl { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreatePaymentDto
    {
        public int InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string? TransactionCode { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountNumber { get; set; }
        public DateOnly PaymentDate { get; set; }
        public int ReceivedByUserId { get; set; }
        public string ReceivedByName { get; set; } = string.Empty;
        public string? ReceiptImageUrl { get; set; }
        public string? Note { get; set; }
    }

    // ===== MaintenanceRequest DTOs =====
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
        public string Priority { get; set; } = string.Empty;
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
        public int? Rating { get; set; }
        public string? Feedback { get; set; }
        public string? RejectedReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateMaintenanceRequestDto
    {
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
    }

    public class AssignMaintenanceRequestDto
    {
        public int AssignedToUserId { get; set; }
        public string? AssignedToName { get; set; }
        public DateTime? ExpectedCompletionDate { get; set; } // Accept DateTime from frontend
        public decimal? EstimatedCost { get; set; }
    }
    
    public class AssignMaintenanceRequest
    {
        public int AssignedToUserId { get; set; }
        public string AssignedToName { get; set; } = string.Empty;
        public DateOnly? ExpectedCompletionDate { get; set; }
        public decimal? EstimatedCost { get; set; }
    }

    public class ResolveMaintenanceRequest
    {
        public string ResolutionNote { get; set; } = string.Empty;
        public decimal? ActualCost { get; set; }
        public string? AfterImageUrls { get; set; }
    }

    public class RejectMaintenanceRequest
    {
        public string RejectedReason { get; set; } = string.Empty;
        public int RejectedByUserId { get; set; }
    }

    public class RateMaintenanceRequest
    {
        public int Rating { get; set; }
        public string? Feedback { get; set; }
    }

    // ===== Notification DTOs =====
    public class NotificationDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public string? IconType { get; set; }
        public bool IsRead { get; set; }
        public DateTime? ReadAt { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? ActionUrl { get; set; }
        public string? IconType { get; set; }
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
    }
}
