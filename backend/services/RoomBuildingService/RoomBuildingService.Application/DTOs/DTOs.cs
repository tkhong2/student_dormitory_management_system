namespace RoomBuildingService.Application.DTOs
{
    public class BuildingDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int TotalFloors { get; set; }
        public int TotalRooms { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ConstructionYear { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool HasElevator { get; set; }
        public bool HasParking { get; set; }
        public bool HasLaundry { get; set; }
        public string? ThumbnailUrl { get; set; }
    }

    public class RoomTypeDto
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal PricePerMonth { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal ElectricityRate { get; set; }
        public decimal WaterRate { get; set; }
        public decimal Area { get; set; }
        public string BedType { get; set; } = string.Empty;
        public bool HasAirConditioner { get; set; }
        public bool HasWaterHeater { get; set; }
        public bool HasPrivateBathroom { get; set; }
        public bool HasWindowView { get; set; }
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool IsActive { get; set; }
        public List<int> AmenityIds { get; set; } = new List<int>();
        public List<AmenityDto> Amenities { get; set; } = new List<AmenityDto>(); // ← THÊM
    }

    public class RoomDto
    {
        public int Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int FloorId { get; set; }
        public int RoomTypeId { get; set; }
        public int BuildingId { get; set; }
        public int FloorNumber { get; set; }
        public string? BuildingName { get; set; }
        public string? RoomTypeName { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CurrentOccupants { get; set; }
        public int MaxOccupants { get; set; }
        public string? AllowedGender { get; set; } // Male / Female / Mixed
        public int AvailableSlots { get; set; } // MaxOccupants - CurrentOccupants
        public string? Orientation { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsLocked { get; set; }
        public string? LockReason { get; set; }
        public string? QRCode { get; set; }
        public DateTime? LastInspectedAt { get; set; }
        public DateTime? AvailableFrom { get; set; }
    }

    public class FloorDto
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public int FloorNumber { get; set; }
        public string Label { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public string? FloorPlanImageUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class AmenityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; }
    }

    public class BuildingAnnouncementDto
    {
        public int Id { get; set; }
        public int? BuildingId { get; set; }
        public string? BuildingName { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public bool IsPinned { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RoomImageDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsCoverImage { get; set; }
        public int SortOrder { get; set; }
    }

    public class RoomInspectionDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public int InspectorUserId { get; set; }
        public string InspectorName { get; set; } = string.Empty;
        public DateOnly InspectionDate { get; set; }
        public string InspectionType { get; set; } = string.Empty;
        public string OverallCondition { get; set; } = string.Empty;
        public string ElectricalStatus { get; set; } = string.Empty;
        public string PlumbingStatus { get; set; } = string.Empty;
        public string FurnitureStatus { get; set; } = string.Empty;
        public string WallStatus { get; set; } = string.Empty;
        public string FloorStatus { get; set; } = string.Empty;
        public decimal? ElectricityReading { get; set; }
        public decimal? WaterReading { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrls { get; set; }
        public DateOnly? NextInspectionDate { get; set; }
        public bool IsApproved { get; set; }
        public int? ApprovedByUserId { get; set; }
        public string? ApprovedByName { get; set; }
        public DateTime? ApprovedAt { get; set; }
        public string? ApprovalNote { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RoomReservationDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string? Note { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public DateTime? ReleasedAt { get; set; }
        public string? ReleaseReason { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class RoomStatusLogDto
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string? RoomNumber { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public int ChangedByUserId { get; set; }
        public string ChangedByName { get; set; } = string.Empty;
        public DateTime ChangedAt { get; set; }
    }

    public class RoomTypeAmenityDto
    {
        public int Id { get; set; }
        public int RoomTypeId { get; set; }
        public int AmenityId { get; set; }
        public string? AmenityName { get; set; }
        public string? AmenityCategory { get; set; }
        public string? AmenityIconUrl { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }

    // Create DTOs
    public class CreateAmenityDto
    {
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class CreateBuildingAnnouncementDto
    {
        public int? BuildingId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = "General";
        public string Priority { get; set; } = "Normal";
        public bool IsPinned { get; set; } = false;
        public DateTime? PublishedAt { get; set; }
        public DateTime? ExpiredAt { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class CreateRoomImageDto
    {
        public int RoomId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsCoverImage { get; set; } = false;
        public int SortOrder { get; set; } = 0;
    }

    public class CreateRoomInspectionDto
    {
        public int RoomId { get; set; }
        public int InspectorUserId { get; set; }
        public string InspectorName { get; set; } = string.Empty;
        public DateOnly InspectionDate { get; set; }
        public string InspectionType { get; set; } = "Routine";
        public string OverallCondition { get; set; } = string.Empty;
        public string ElectricalStatus { get; set; } = "OK";
        public string PlumbingStatus { get; set; } = "OK";
        public string FurnitureStatus { get; set; } = "OK";
        public string WallStatus { get; set; } = "OK";
        public string FloorStatus { get; set; } = "OK";
        public decimal? ElectricityReading { get; set; }
        public decimal? WaterReading { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrls { get; set; }
        public DateOnly? NextInspectionDate { get; set; }
    }

    public class CreateRoomReservationDto
    {
        public int RoomId { get; set; }
        public int ApplicationId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string? Note { get; set; }
        public int CreatedByUserId { get; set; }
        public string CreatedByName { get; set; } = string.Empty;
    }

    public class CreateRoomTypeAmenityDto
    {
        public int RoomTypeId { get; set; }
        public int AmenityId { get; set; }
        public int Quantity { get; set; } = 1;
        public string? Note { get; set; }
    }

    // ============ Room Amenity Inspection DTOs ============
    
    public class RoomAmenityInspectionDto
    {
        public int Id { get; set; }
        public int RoomInspectionId { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int? AmenityId { get; set; }
        public string AmenityName { get; set; } = string.Empty;
        public string AmenityCategory { get; set; } = string.Empty;
        public string Condition { get; set; } = "Good";
        public string? IssueDescription { get; set; }
        public string? RecommendedAction { get; set; }
        public string? ImageUrls { get; set; }
        public int? MaintenanceRequestId { get; set; }
        public bool NeedMaintenance { get; set; }
        public bool MaintenanceRequestCreated { get; set; }
        public string Priority { get; set; } = "Medium";
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateRoomAmenityInspectionDto
    {
        public int RoomInspectionId { get; set; }
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int? AmenityId { get; set; }
        public string AmenityName { get; set; } = string.Empty;
        public string AmenityCategory { get; set; } = string.Empty;
        public string Condition { get; set; } = "Good";
        public string? IssueDescription { get; set; }
        public string? RecommendedAction { get; set; }
        public string? ImageUrls { get; set; }
        public bool NeedMaintenance { get; set; } = false;
        public string Priority { get; set; } = "Medium";
        public string? Notes { get; set; }
    }

    public class UpdateRoomAmenityInspectionDto
    {
        public string Condition { get; set; } = "Good";
        public string? IssueDescription { get; set; }
        public string? RecommendedAction { get; set; }
        public string? ImageUrls { get; set; }
        public bool NeedMaintenance { get; set; }
        public string Priority { get; set; } = "Medium";
        public string? Notes { get; set; }
    }

    // ============ Public Facility DTOs ============
    
    public class PublicFacilityDto
    {
        public int Id { get; set; }
        public int? BuildingId { get; set; }
        public string? BuildingName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string FacilityId { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerEmail { get; set; }
        public string? SocialLinks { get; set; }
        public string? OperatingHours { get; set; }
        public bool IsBookingRequired { get; set; }
        public int? MaxCapacity { get; set; }
        public decimal? FeePerHour { get; set; }
        public decimal? FeePerSession { get; set; }
        public string? FeeNote { get; set; }
        public string? EventSchedule { get; set; }
        public bool IsVisibleOnHomepage { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsFeatured { get; set; }
        public int TotalUsageCount { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }

    public class CreatePublicFacilityDto
    {
        public int? BuildingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string FacilityId { get; set; } = string.Empty;
        public string Status { get; set; } = "Active";
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerEmail { get; set; }
        public string? SocialLinks { get; set; }
        public string? OperatingHours { get; set; }
        public bool IsBookingRequired { get; set; } = false;
        public int? MaxCapacity { get; set; }
        public decimal? FeePerHour { get; set; }
        public decimal? FeePerSession { get; set; }
        public string? FeeNote { get; set; }
        public string? EventSchedule { get; set; }
        public bool IsVisibleOnHomepage { get; set; } = true;
        public int DisplayOrder { get; set; } = 0;
        public bool IsFeatured { get; set; } = false;
    }

    public class UpdatePublicFacilityDto
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? BrandName { get; set; }
        public string? FacilityId { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ImageUrl { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerEmail { get; set; }
        public string? SocialLinks { get; set; }
        public string? OperatingHours { get; set; }
        public bool? IsBookingRequired { get; set; }
        public int? MaxCapacity { get; set; }
        public decimal? FeePerHour { get; set; }
        public decimal? FeePerSession { get; set; }
        public string? FeeNote { get; set; }
        public string? EventSchedule { get; set; }
        public bool? IsVisibleOnHomepage { get; set; }
        public int? DisplayOrder { get; set; }
        public bool? IsFeatured { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }

    // ============ Facility Booking DTOs ============
    
    public class FacilityBookingDto
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentPhone { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string Status { get; set; } = "Pending";
        public string? Purpose { get; set; }
        public decimal? Fee { get; set; }
        public bool IsPaid { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancelledAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateFacilityBookingDto
    {
        public int FacilityId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string StudentPhone { get; set; } = string.Empty;
        public DateOnly BookingDate { get; set; }
        public TimeOnly? StartTime { get; set; }
        public TimeOnly? EndTime { get; set; }
        public string? Purpose { get; set; }
    }

    // ============ Facility Review DTOs ============
    
    public class FacilityReviewDto
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsApproved { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateFacilityReviewDto
    {
        public int FacilityId { get; set; }
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? ImageUrl { get; set; }
    }

    // ============ Shared Utility DTOs ============
    
    public class SharedUtilityDto
    {
        public int Id { get; set; }
        public int BuildingId { get; set; }
        public string? BuildingName { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string UtilityId { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
        public string? Location { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerEmail { get; set; }
        public string? SocialLinks { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? FeePerUse { get; set; }
        public string? OperatingHours { get; set; }
        public string? UsageInstructions { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? InstallationDate { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
        public int? WarrantyMonths { get; set; }
        public DateTime? WarrantyExpiry { get; set; }
        public int TotalUsageCount { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateSharedUtilityDto
    {
        public int BuildingId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? Brand { get; set; }
        public string UtilityId { get; set; } = string.Empty;
        public string Status { get; set; } = "Available";
        public string? Location { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerEmail { get; set; }
        public string? SocialLinks { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? FeePerUse { get; set; }
        public string? OperatingHours { get; set; }
        public string? UsageInstructions { get; set; }
        public DateTime? PurchaseDate { get; set; }
        public DateTime? InstallationDate { get; set; }
        public int? WarrantyMonths { get; set; }
    }

    public class UpdateSharedUtilityDto
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public string? Brand { get; set; }
        public string? Status { get; set; }
        public string? Location { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ManagerEmail { get; set; }
        public string? SocialLinks { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? FeePerUse { get; set; }
        public string? OperatingHours { get; set; }
        public string? UsageInstructions { get; set; }
        public DateTime? LastMaintenanceDate { get; set; }
        public DateTime? NextMaintenanceDate { get; set; }
    }

    // ============ Utility Event DTOs ============
    
    public class UtilityEventDto
    {
        public int Id { get; set; }
        public int SharedUtilityId { get; set; }
        public string? UtilityName { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string Status { get; set; } = "Scheduled";
        public decimal? EstimatedCost { get; set; }
        public decimal? ActualCost { get; set; }
        public int? PerformedByUserId { get; set; }
        public string? PerformedByName { get; set; }
        public string? TechnicianCompany { get; set; }
        public string? TechnicianContact { get; set; }
        public int? MaintenanceRequestId { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrls { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUtilityEventDto
    {
        public int SharedUtilityId { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime EventDate { get; set; }
        public decimal? EstimatedCost { get; set; }
        public int? PerformedByUserId { get; set; }
        public string? PerformedByName { get; set; }
        public string? TechnicianCompany { get; set; }
        public string? TechnicianContact { get; set; }
        public int? MaintenanceRequestId { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateUtilityEventDto
    {
        public string? Status { get; set; }
        public DateTime? CompletedDate { get; set; }
        public decimal? ActualCost { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrls { get; set; }
    }

    // ============ Utility Usage Log DTOs ============
    
    public class UtilityUsageLogDto
    {
        public int Id { get; set; }
        public int SharedUtilityId { get; set; }
        public string? UtilityName { get; set; }
        public int? StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? RoomNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? FeeCharged { get; set; }
        public bool IsPaid { get; set; }
        public int? InvoiceId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateUtilityUsageLogDto
    {
        public int SharedUtilityId { get; set; }
        public int? StudentId { get; set; }
        public string? StudentName { get; set; }
        public string? RoomNumber { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public decimal? FeeCharged { get; set; }
        public string? Notes { get; set; }
    }

    public class MarkPaidFromWebhookDto
    {
        public string? TransactionCode { get; set; }
        public decimal Amount { get; set; }
    }
}
