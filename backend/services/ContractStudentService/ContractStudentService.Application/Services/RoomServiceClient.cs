using System.Net.Http.Json;

namespace ContractStudentService.Application.Services;

public interface IRoomServiceClient
{
    Task<bool> UpdateRoomOccupancyAsync(int roomId, int increment);
    Task<RoomInfoDto?> GetRoomInfoAsync(int roomId);
    Task<RoomInfoDto?> GetRoomByIdAsync(int roomId);
    Task<bool> UpdateRoomGenderAsync(int roomId, string gender);
    Task<BuildingInfoDto?> GetBuildingByIdAsync(int buildingId);
}

public class RoomServiceClient : IRoomServiceClient
{
    private readonly HttpClient _httpClient;

    public RoomServiceClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    /// Tăng/giảm số người trong phòng và tự động cập nhật trạng thái
    /// </summary>
    /// <param name="roomId">ID phòng</param>
    /// <param name="increment">Số người thay đổi (+1: thêm, -1: bớt)</param>
    public async Task<bool> UpdateRoomOccupancyAsync(int roomId, int increment)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"/api/rooms/{roomId}/occupancy",
                new { increment });

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Lấy thông tin phòng (capacity, current occupants, status)
    /// </summary>
    public async Task<RoomInfoDto?> GetRoomInfoAsync(int roomId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/rooms/{roomId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<RoomInfoDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Alias for GetRoomInfoAsync
    /// </summary>
    public async Task<RoomInfoDto?> GetRoomByIdAsync(int roomId)
    {
        return await GetRoomInfoAsync(roomId);
    }

    /// <summary>
    /// Cập nhật giới tính được phép cho phòng
    /// </summary>
    public async Task<bool> UpdateRoomGenderAsync(int roomId, string gender)
    {
        try
        {
            var response = await _httpClient.PutAsJsonAsync(
                $"/api/rooms/{roomId}/gender",
                new { allowedGender = gender });

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Lấy thông tin tòa nhà (tên, giới tính)
    /// </summary>
    public async Task<BuildingInfoDto?> GetBuildingByIdAsync(int buildingId)
    {
        try
        {
            var response = await _httpClient.GetAsync($"/api/buildings/{buildingId}");
            
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<BuildingInfoDto>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}

public class RoomInfoDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int BuildingId { get; set; }
    public int CurrentOccupants { get; set; }
    public int MaxOccupants { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? AllowedGender { get; set; }
}

public class BuildingInfoDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty; // Male / Female / Mixed
}
