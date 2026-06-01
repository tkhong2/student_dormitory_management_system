namespace RoomBuildingService.Domain.Entities;

/// <summary>
/// Ảnh phòng (gallery)
/// </summary>
public class RoomImage : BaseEntity
{
    public int RoomId { get; set; }
    public string ImageUrl { get; set; } = null!;
    public string? Caption { get; set; }
    public bool IsCoverImage { get; set; } = false;
    public int SortOrder { get; set; } = 0;

    // Navigation
    public Room Room { get; set; } = null!;
}
