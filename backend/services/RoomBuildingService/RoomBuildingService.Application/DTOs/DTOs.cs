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
}
