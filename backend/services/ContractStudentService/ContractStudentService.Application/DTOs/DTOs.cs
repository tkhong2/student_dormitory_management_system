namespace ContractStudentService.Application.DTOs
{
    // ===== Student DTOs =====
    public class StudentDto
    {
        public int Id { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public int AcademicYear { get; set; }
        public string? ClassCode { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? Nationality { get; set; }
        public string? BloodType { get; set; }
        public string? HealthInsuranceNumber { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public DateOnly IdentityCardIssuedDate { get; set; }
        public string IdentityCardIssuedPlace { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public string? PermanentProvince { get; set; }
        public string? TemporaryAddress { get; set; }
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string EmergencyContactRelation { get; set; } = string.Empty;
        public string? EmergencyContactAddress { get; set; }
        public string? AvatarUrl { get; set; }
        public int ProfileCompletionPct { get; set; }
        public bool IsActive { get; set; }
        public string? Notes { get; set; }
        public int? UserId { get; set; }
    }

    public class CreateStudentDto
    {
        public string StudentCode { get; set; } = string.Empty;
        public string Faculty { get; set; } = string.Empty;
        public string Major { get; set; } = string.Empty;
        public int AcademicYear { get; set; }
        public string? ClassCode { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateOnly DateOfBirth { get; set; }
        public string? PlaceOfBirth { get; set; }
        public string? Ethnicity { get; set; }
        public string? Religion { get; set; }
        public string? Nationality { get; set; }
        public string? BloodType { get; set; }
        public string? HealthInsuranceNumber { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IdentityCard { get; set; } = string.Empty;
        public DateOnly IdentityCardIssuedDate { get; set; }
        public string IdentityCardIssuedPlace { get; set; } = string.Empty;
        public string PermanentAddress { get; set; } = string.Empty;
        public string? PermanentProvince { get; set; }
        public string? TemporaryAddress { get; set; }
        public string EmergencyContactName { get; set; } = string.Empty;
        public string EmergencyContactPhone { get; set; } = string.Empty;
        public string EmergencyContactRelation { get; set; } = string.Empty;
        public string? EmergencyContactAddress { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public int? UserId { get; set; }
    }

    // ===== StudentDocument DTOs =====
    public class StudentDocumentDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public bool IsVerified { get; set; }
        public int? VerifiedByUserId { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateStudentDocumentDto
    {
        public int StudentId { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string DocumentName { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public long FileSizeBytes { get; set; }
        public string? Notes { get; set; }
    }

    // ===== RoomApplication DTOs =====
    public class RoomApplicationDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public int PreferredBuildingId { get; set; }
        public string PreferredBuildingName { get; set; } = string.Empty;
        public int PreferredRoomTypeId { get; set; }
        public string PreferredRoomTypeName { get; set; } = string.Empty;
        public decimal PreferredRoomPrice { get; set; }
        public DateOnly RequestedStartDate { get; set; }
        public DateOnly RequestedEndDate { get; set; }
        public string? SpecialRequirements { get; set; }
        public string? Note { get; set; }
        public bool IsLocalStudent { get; set; }
        public int Priority { get; set; }
        public string? AttachedDocumentUrls { get; set; }
        public string Status { get; set; } = string.Empty;
        public int? ReviewedByUserId { get; set; }
        public string? ReviewedByName { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectReason { get; set; }
        public int? AssignedRoomId { get; set; }
        public string? AssignedRoomNumber { get; set; }
        public string? AssignedBuildingName { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? CancelledReason { get; set; }
        public bool CancelledBySelf { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRoomApplicationDto
    {
        public int StudentId { get; set; }
        public int PreferredBuildingId { get; set; }
        public string PreferredBuildingName { get; set; } = string.Empty;
        public int PreferredRoomTypeId { get; set; }
        public string PreferredRoomTypeName { get; set; } = string.Empty;
        public decimal PreferredRoomPrice { get; set; }
        public DateOnly RequestedStartDate { get; set; }
        public DateOnly RequestedEndDate { get; set; }
        public string? SpecialRequirements { get; set; }
        public string? Note { get; set; }
        public bool IsLocalStudent { get; set; } = false;
        public int Priority { get; set; } = 0;
        public string? AttachedDocumentUrls { get; set; }
    }

    // ===== Contract DTOs =====
    public class ContractDto
    {
        public int Id { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public int ApplicationId { get; set; }
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
        public decimal DepositAmount { get; set; }
        public bool IsDepositPaid { get; set; }
        public DateTime? DepositPaidAt { get; set; }
        public decimal ElectricityRate { get; set; }
        public decimal WaterRate { get; set; }
        public int PaymentDueDay { get; set; }
        public string? WitnessName { get; set; }
        public string? WitnessTitle { get; set; }
        public DateTime? SignedAt { get; set; }
        public string? SignedImageUrl { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? TerminatedAt { get; set; }
        public string? TerminatedReason { get; set; }
        public int? TerminatedByUserId { get; set; }
        public decimal? DepositReturnedAmount { get; set; }
        public DateTime? DepositReturnedAt { get; set; }
        public string? DepositDeductionReason { get; set; }
        public int CreatedByUserId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateContractDto
    {
        public int StudentId { get; set; }
        public int ApplicationId { get; set; }
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
        public decimal DepositAmount { get; set; }
        public decimal ElectricityRate { get; set; }
        public decimal WaterRate { get; set; }
        public int PaymentDueDay { get; set; } = 5;
        public string? WitnessName { get; set; }
        public string? WitnessTitle { get; set; }
        public int CreatedByUserId { get; set; }
        public string? Notes { get; set; }
    }

    // ===== ContractExtension DTOs =====
    public class ContractExtensionDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string? ContractCode { get; set; }
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public DateOnly OldEndDate { get; set; }
        public DateOnly NewEndDate { get; set; }
        public string? Reason { get; set; }
        public int ApprovedByUserId { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;
        public DateTime ApprovedAt { get; set; }
    }

    public class CreateContractExtensionDto
    {
        public int ContractId { get; set; }
        public DateOnly NewEndDate { get; set; }
        public string? Reason { get; set; }
        public int ApprovedByUserId { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;
    }

    // ===== RoomTransfer DTOs =====
    public class RoomTransferDto
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public int StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? StudentCode { get; set; }
        public int CurrentRoomId { get; set; }
        public string CurrentRoomNumber { get; set; } = string.Empty;
        public int? RequestedRoomTypeId { get; set; }
        public string? RequestedRoomTypeName { get; set; }
        public int? RequestedBuildingId { get; set; }
        public string? RequestedBuildingName { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int? NewRoomId { get; set; }
        public string? NewRoomNumber { get; set; }
        public DateOnly? TransferDate { get; set; }
        public int? ReviewedByUserId { get; set; }
        public string? ReviewedByName { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string? RejectReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRoomTransferDto
    {
        public int ContractId { get; set; }
        public int StudentId { get; set; }
        public int CurrentRoomId { get; set; }
        public string CurrentRoomNumber { get; set; } = string.Empty;
        public int? RequestedRoomTypeId { get; set; }
        public string? RequestedRoomTypeName { get; set; }
        public int? RequestedBuildingId { get; set; }
        public string? RequestedBuildingName { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
