using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;
using RoomBuildingService.Infrastructure.Persistence;

namespace RoomBuildingService.Infrastructure.Repositories
{
    public class BuildingRepository : IBuildingRepository
    {
        private readonly AppDbContext _context;
        public BuildingRepository(AppDbContext context) => _context = context;

        public async Task<Building?> GetByIdAsync(Guid id) => await _context.Buildings.FindAsync(id);
        public async Task<IEnumerable<Building>> GetAllAsync() => await _context.Buildings.ToListAsync();
        public async Task AddAsync(Building building) { await _context.Buildings.AddAsync(building); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Building building) { _context.Buildings.Update(building); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(Building building) { _context.Buildings.Remove(building); await _context.SaveChangesAsync(); }
    }

    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly AppDbContext _context;
        public RoomTypeRepository(AppDbContext context) => _context = context;

        public async Task<RoomType?> GetByIdAsync(Guid id) => await _context.RoomTypes.FindAsync(id);
        public async Task<IEnumerable<RoomType>> GetAllAsync() => await _context.RoomTypes.ToListAsync();
        public async Task AddAsync(RoomType roomType) { await _context.RoomTypes.AddAsync(roomType); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(RoomType roomType) { _context.RoomTypes.Update(roomType); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(RoomType roomType) { _context.RoomTypes.Remove(roomType); await _context.SaveChangesAsync(); }
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;
        public RoomRepository(AppDbContext context) => _context = context;

        public async Task<Room?> GetByIdAsync(Guid id) => await _context.Rooms.Include(r => r.RoomType).Include(r => r.Building).FirstOrDefaultAsync(r => r.Id == id);
        public async Task<IEnumerable<Room>> GetAllAsync() => await _context.Rooms.Include(r => r.RoomType).Include(r => r.Building).ToListAsync();
        public async Task<IEnumerable<Room>> GetByBuildingIdAsync(Guid buildingId) => await _context.Rooms.Where(r => r.BuildingId == buildingId).Include(r => r.RoomType).ToListAsync();
        public async Task AddAsync(Room room) { await _context.Rooms.AddAsync(room); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Room room) { _context.Rooms.Update(room); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(Room room) { _context.Rooms.Remove(room); await _context.SaveChangesAsync(); }
    }
}
