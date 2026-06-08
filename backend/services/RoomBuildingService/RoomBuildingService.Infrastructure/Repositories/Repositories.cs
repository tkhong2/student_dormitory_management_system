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

        public async Task<Building?> GetByIdAsync(int id) => await _context.Buildings.FindAsync(id);
        public async Task<IEnumerable<Building>> GetAllAsync() => await _context.Buildings.ToListAsync();
        public async Task AddAsync(Building building) { await _context.Buildings.AddAsync(building); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Building building) { _context.Buildings.Update(building); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(Building building) { _context.Buildings.Remove(building); await _context.SaveChangesAsync(); }
    }

    public class RoomTypeRepository : IRoomTypeRepository
    {
        private readonly AppDbContext _context;
        public RoomTypeRepository(AppDbContext context) => _context = context;

        public async Task<RoomType?> GetByIdAsync(int id) => await _context.RoomTypes.FindAsync(id);
        public async Task<IEnumerable<RoomType>> GetAllAsync() => await _context.RoomTypes.ToListAsync();
        public async Task AddAsync(RoomType roomType) { await _context.RoomTypes.AddAsync(roomType); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(RoomType roomType) { _context.RoomTypes.Update(roomType); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(RoomType roomType) { _context.RoomTypes.Remove(roomType); await _context.SaveChangesAsync(); }
    }

    public class RoomRepository : IRoomRepository
    {
        private readonly AppDbContext _context;
        public RoomRepository(AppDbContext context) => _context = context;

        public async Task<Room?> GetByIdAsync(int id) => await _context.Rooms
            .Include(r => r.RoomType)
            .Include(r => r.Floor)
                .ThenInclude(f => f.Building)
            .Include(r => r.Images)
            .FirstOrDefaultAsync(r => r.Id == id);

        public async Task<IEnumerable<Room>> GetAllAsync() => await _context.Rooms
            .AsNoTracking() // Tối ưu performance khi chỉ đọc dữ liệu
            .Include(r => r.RoomType)
            .Include(r => r.Floor)
                .ThenInclude(f => f.Building)
            .Include(r => r.Images)
            .AsSplitQuery() // Tránh cartesian explosion với nhiều includes
            .ToListAsync();

        public async Task<IEnumerable<Room>> GetByBuildingIdAsync(int buildingId) => await _context.Rooms
            .AsNoTracking()
            .Include(r => r.RoomType)
            .Include(r => r.Floor)
                .ThenInclude(f => f.Building)
            .Include(r => r.Images)
            .Where(r => r.Floor.BuildingId == buildingId)
            .AsSplitQuery()
            .ToListAsync();

        public async Task AddAsync(Room room) { await _context.Rooms.AddAsync(room); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Room room) { _context.Rooms.Update(room); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(Room room) { _context.Rooms.Remove(room); await _context.SaveChangesAsync(); }
    }

    public class FloorRepository : IFloorRepository
    {
        private readonly AppDbContext _context;
        public FloorRepository(AppDbContext context) => _context = context;

        public async Task<Floor?> GetByIdAsync(int id) => await _context.Floors.FindAsync(id);
        public async Task<IEnumerable<Floor>> GetAllAsync() => await _context.Floors.ToListAsync();
        public async Task<IEnumerable<Floor>> GetByBuildingIdAsync(int buildingId) => await _context.Floors.Where(f => f.BuildingId == buildingId).ToListAsync();
        public async Task AddAsync(Floor floor) { await _context.Floors.AddAsync(floor); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Floor floor) { _context.Floors.Update(floor); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(Floor floor) { _context.Floors.Remove(floor); await _context.SaveChangesAsync(); }
    }

    public class AmenityRepository : IAmenityRepository
    {
        private readonly AppDbContext _context;
        public AmenityRepository(AppDbContext context) => _context = context;

        public async Task<Amenity?> GetByIdAsync(int id) => await _context.Amenities.FindAsync(id);
        public async Task<IEnumerable<Amenity>> GetAllAsync() => await _context.Amenities.ToListAsync();
        public async Task<IEnumerable<Amenity>> GetActivesAsync() => await _context.Amenities.Where(a => a.IsActive).ToListAsync();
        public async Task AddAsync(Amenity amenity) { await _context.Amenities.AddAsync(amenity); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(Amenity amenity) { _context.Amenities.Update(amenity); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(Amenity amenity) { _context.Amenities.Remove(amenity); await _context.SaveChangesAsync(); }
    }

    public class BuildingAnnouncementRepository : IBuildingAnnouncementRepository
    {
        private readonly AppDbContext _context;
        public BuildingAnnouncementRepository(AppDbContext context) => _context = context;

        public async Task<BuildingAnnouncement?> GetByIdAsync(int id) => await _context.BuildingAnnouncements.Include(a => a.Building).FirstOrDefaultAsync(a => a.Id == id);
        public async Task<IEnumerable<BuildingAnnouncement>> GetAllAsync() => await _context.BuildingAnnouncements.Include(a => a.Building).OrderByDescending(a => a.IsPinned).ThenByDescending(a => a.PublishedAt).ToListAsync();
        public async Task<IEnumerable<BuildingAnnouncement>> GetByBuildingIdAsync(int? buildingId) => await _context.BuildingAnnouncements.Include(a => a.Building).Where(a => a.BuildingId == buildingId).OrderByDescending(a => a.IsPinned).ThenByDescending(a => a.PublishedAt).ToListAsync();
        public async Task<IEnumerable<BuildingAnnouncement>> GetPublishedAsync() => await _context.BuildingAnnouncements.Include(a => a.Building).Where(a => a.PublishedAt != null && a.PublishedAt <= DateTime.UtcNow && (a.ExpiredAt == null || a.ExpiredAt > DateTime.UtcNow)).OrderByDescending(a => a.IsPinned).ThenByDescending(a => a.PublishedAt).ToListAsync();
        public async Task AddAsync(BuildingAnnouncement announcement) { await _context.BuildingAnnouncements.AddAsync(announcement); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(BuildingAnnouncement announcement) { _context.BuildingAnnouncements.Update(announcement); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(BuildingAnnouncement announcement) { _context.BuildingAnnouncements.Remove(announcement); await _context.SaveChangesAsync(); }
    }

    public class RoomImageRepository : IRoomImageRepository
    {
        private readonly AppDbContext _context;
        public RoomImageRepository(AppDbContext context) => _context = context;

        public async Task<RoomImage?> GetByIdAsync(int id) => await _context.RoomImages.FindAsync(id);
        public async Task<IEnumerable<RoomImage>> GetByRoomIdAsync(int roomId) => await _context.RoomImages.Where(i => i.RoomId == roomId).OrderBy(i => i.SortOrder).ToListAsync();
        public async Task AddAsync(RoomImage roomImage) { await _context.RoomImages.AddAsync(roomImage); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(RoomImage roomImage) { _context.RoomImages.Update(roomImage); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(RoomImage roomImage) { _context.RoomImages.Remove(roomImage); await _context.SaveChangesAsync(); }
    }

    public class RoomInspectionRepository : IRoomInspectionRepository
    {
        private readonly AppDbContext _context;
        public RoomInspectionRepository(AppDbContext context) => _context = context;

        public async Task<RoomInspection?> GetByIdAsync(int id) => await _context.RoomInspections.Include(i => i.Room).FirstOrDefaultAsync(i => i.Id == id);
        public async Task<IEnumerable<RoomInspection>> GetAllAsync() => await _context.RoomInspections.Include(i => i.Room).OrderByDescending(i => i.InspectionDate).ToListAsync();
        public async Task<IEnumerable<RoomInspection>> GetByRoomIdAsync(int roomId) => await _context.RoomInspections.Where(i => i.RoomId == roomId).OrderByDescending(i => i.InspectionDate).ToListAsync();
        public async Task<IEnumerable<RoomInspection>> GetPendingApprovalsAsync() => await _context.RoomInspections.Include(i => i.Room).Where(i => !i.IsApproved).OrderBy(i => i.InspectionDate).ToListAsync();
        public async Task AddAsync(RoomInspection inspection) { await _context.RoomInspections.AddAsync(inspection); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(RoomInspection inspection) { _context.RoomInspections.Update(inspection); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(RoomInspection inspection) { _context.RoomInspections.Remove(inspection); await _context.SaveChangesAsync(); }
    }

    public class RoomReservationRepository : IRoomReservationRepository
    {
        private readonly AppDbContext _context;
        public RoomReservationRepository(AppDbContext context) => _context = context;

        public async Task<RoomReservation?> GetByIdAsync(int id) => await _context.RoomReservations.Include(r => r.Room).FirstOrDefaultAsync(r => r.Id == id);
        public async Task<IEnumerable<RoomReservation>> GetAllAsync() => await _context.RoomReservations.Include(r => r.Room).OrderByDescending(r => r.CreatedAt).ToListAsync();
        public async Task<IEnumerable<RoomReservation>> GetByRoomIdAsync(int roomId) => await _context.RoomReservations.Where(r => r.RoomId == roomId).OrderByDescending(r => r.CreatedAt).ToListAsync();
        public async Task<IEnumerable<RoomReservation>> GetActiveReservationsAsync() => await _context.RoomReservations.Include(r => r.Room).Where(r => r.Status == "Holding" && r.ExpiresAt > DateTime.UtcNow).ToListAsync();
        public async Task<RoomReservation?> GetByApplicationIdAsync(int applicationId) => await _context.RoomReservations.Include(r => r.Room).FirstOrDefaultAsync(r => r.ApplicationId == applicationId && r.Status == "Holding");
        public async Task AddAsync(RoomReservation reservation) { await _context.RoomReservations.AddAsync(reservation); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(RoomReservation reservation) { _context.RoomReservations.Update(reservation); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(RoomReservation reservation) { _context.RoomReservations.Remove(reservation); await _context.SaveChangesAsync(); }
    }

    public class RoomStatusLogRepository : IRoomStatusLogRepository
    {
        private readonly AppDbContext _context;
        public RoomStatusLogRepository(AppDbContext context) => _context = context;

        public async Task<RoomStatusLog?> GetByIdAsync(int id) => await _context.RoomStatusLogs.FindAsync(id);
        public async Task<IEnumerable<RoomStatusLog>> GetByRoomIdAsync(int roomId) => await _context.RoomStatusLogs.Where(l => l.RoomId == roomId).OrderByDescending(l => l.ChangedAt).ToListAsync();
        public async Task AddAsync(RoomStatusLog log) { await _context.RoomStatusLogs.AddAsync(log); await _context.SaveChangesAsync(); }
    }

    public class RoomTypeAmenityRepository : IRoomTypeAmenityRepository
    {
        private readonly AppDbContext _context;
        public RoomTypeAmenityRepository(AppDbContext context) => _context = context;

        public async Task<RoomTypeAmenity?> GetByIdAsync(int id) => await _context.RoomTypeAmenities.Include(rta => rta.Amenity).Include(rta => rta.RoomType).FirstOrDefaultAsync(rta => rta.Id == id);
        public async Task<IEnumerable<RoomTypeAmenity>> GetByRoomTypeIdAsync(int roomTypeId) => await _context.RoomTypeAmenities.Include(rta => rta.Amenity).Where(rta => rta.RoomTypeId == roomTypeId).ToListAsync();
        public async Task<IEnumerable<RoomTypeAmenity>> GetByAmenityIdAsync(int amenityId) => await _context.RoomTypeAmenities.Include(rta => rta.RoomType).Where(rta => rta.AmenityId == amenityId).ToListAsync();
        public async Task AddAsync(RoomTypeAmenity roomTypeAmenity) { await _context.RoomTypeAmenities.AddAsync(roomTypeAmenity); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(RoomTypeAmenity roomTypeAmenity) { _context.RoomTypeAmenities.Update(roomTypeAmenity); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(RoomTypeAmenity roomTypeAmenity) { _context.RoomTypeAmenities.Remove(roomTypeAmenity); await _context.SaveChangesAsync(); }
    }
}
