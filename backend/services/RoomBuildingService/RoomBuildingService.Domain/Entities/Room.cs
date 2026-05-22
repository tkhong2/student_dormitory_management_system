using RoomBuildingService.Domain.Enums;

namespace RoomBuildingService.Domain.Entities
{
    public class Room
    {
        public Guid Id { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public int Floor { get; set; }
        public Guid BuildingId { get; set; }
        public Guid RoomTypeId { get; set; }
        public RoomStatus Status { get; set; } = RoomStatus.Available;
        public int CurrentOccupancy { get; set; } = 0;

        // Navigation properties
        public Building? Building { get; set; }
        public RoomType? RoomType { get; set; }
    }
}
