namespace RoomBuildingService.Domain.Entities
{
    public class RoomType
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;

        // Navigation properties
        public ICollection<Room> Rooms { get; set; } = new List<Room>();
    }
}
