using System.Net.Http.Json;

namespace ContractStudentService.Application.Services;

public interface IRoomServiceClient
{
    Task<bool> UpdateRoomOccupancyAsync(int roomId, int increment);
    Task<RoomInfoDto?> GetRoomInfoAsync(int roomId);
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
}

public class RoomInfoDto
{
    public int Id { get; set; }
    public string RoomNumber { get; set; } = string.Empty;
    public int CurrentOccupants { get; set; }
    public int MaxOccupants { get; set; }
    public string Status { get; set; } = string.Empty;
}
