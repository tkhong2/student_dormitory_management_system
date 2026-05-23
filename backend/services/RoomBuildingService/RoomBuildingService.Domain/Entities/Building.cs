using RoomBuildingService.Domain.Enums;

namespace RoomBuildingService.Domain.Entities
{
    public class Building
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfFloors { get; set; }
        public BuildingType Type { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
