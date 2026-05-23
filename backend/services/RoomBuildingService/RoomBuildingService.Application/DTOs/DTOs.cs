namespace RoomBuildingService.Application.DTOs
{
    public class BuildingDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfFloors { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }

    public class RoomTypeDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    public class RoomDto
    {
        public Guid Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int Floor { get; set; }
        public Guid BuildingId { get; set; }
        public Guid RoomTypeId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CurrentOccupancy { get; set; }
        public string? ImageUrl { get; set; }
    }
}
