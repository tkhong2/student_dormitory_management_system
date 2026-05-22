using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.Application.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building?> GetByIdAsync(Guid id);
        Task<IEnumerable<Building>> GetAllAsync();
        Task AddAsync(Building building);
        Task UpdateAsync(Building building);
        Task DeleteAsync(Building building);
    }

    public interface IRoomTypeRepository
    {
        Task<RoomType?> GetByIdAsync(Guid id);
        Task<IEnumerable<RoomType>> GetAllAsync();
        Task AddAsync(RoomType roomType);
        Task UpdateAsync(RoomType roomType);
        Task DeleteAsync(RoomType roomType);
    }

    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(Guid id);
        Task<IEnumerable<Room>> GetAllAsync();
        Task<IEnumerable<Room>> GetByBuildingIdAsync(Guid buildingId);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(Room room);
    }
}
