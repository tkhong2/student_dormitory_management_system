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

        public async Task<IEnumerable<RoomTypeAmenity>> GetAllAsync() => await _context.RoomTypeAmenities.Include(rta => rta.Amenity).Include(rta => rta.RoomType).ToListAsync();
        public async Task<RoomTypeAmenity?> GetByIdAsync(int id) => await _context.RoomTypeAmenities.Include(rta => rta.Amenity).Include(rta => rta.RoomType).FirstOrDefaultAsync(rta => rta.Id == id);
        public async Task<IEnumerable<RoomTypeAmenity>> GetByRoomTypeIdAsync(int roomTypeId) => await _context.RoomTypeAmenities.Include(rta => rta.Amenity).Where(rta => rta.RoomTypeId == roomTypeId).ToListAsync();
        public async Task<IEnumerable<RoomTypeAmenity>> GetByAmenityIdAsync(int amenityId) => await _context.RoomTypeAmenities.Include(rta => rta.RoomType).Where(rta => rta.AmenityId == amenityId).ToListAsync();
        public async Task AddAsync(RoomTypeAmenity roomTypeAmenity) { await _context.RoomTypeAmenities.AddAsync(roomTypeAmenity); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(RoomTypeAmenity roomTypeAmenity) { _context.RoomTypeAmenities.Update(roomTypeAmenity); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(RoomTypeAmenity roomTypeAmenity) { _context.RoomTypeAmenities.Remove(roomTypeAmenity); await _context.SaveChangesAsync(); }
    }

    public class RoomAmenityInspectionRepository : IRoomAmenityInspectionRepository
    {
        private readonly AppDbContext _context;
        public RoomAmenityInspectionRepository(AppDbContext context) => _context = context;

        public async Task<RoomAmenityInspection?> GetByIdAsync(int id) => await _context.RoomAmenityInspections.FindAsync(id);
        public async Task<IEnumerable<RoomAmenityInspection>> GetAllAsync() => await _context.RoomAmenityInspections.OrderByDescending(i => i.CreatedAt).ToListAsync();
        public async Task<IEnumerable<RoomAmenityInspection>> GetByRoomInspectionIdAsync(int roomInspectionId) => await _context.RoomAmenityInspections.Where(i => i.RoomInspectionId == roomInspectionId).ToListAsync();
        public async Task<IEnumerable<RoomAmenityInspection>> GetByRoomIdAsync(int roomId) => await _context.RoomAmenityInspections.Where(i => i.RoomId == roomId).OrderByDescending(i => i.CreatedAt).ToListAsync();
        public async Task<IEnumerable<RoomAmenityInspection>> GetNeedMaintenanceAsync() => await _context.RoomAmenityInspections.Where(i => i.NeedMaintenance && !i.MaintenanceRequestCreated).OrderBy(i => i.Priority).ToListAsync();
        public async Task AddAsync(RoomAmenityInspection inspection) { await _context.RoomAmenityInspections.AddAsync(inspection); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(RoomAmenityInspection inspection) { _context.RoomAmenityInspections.Update(inspection); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(RoomAmenityInspection inspection) { _context.RoomAmenityInspections.Remove(inspection); await _context.SaveChangesAsync(); }
    }

    public class PublicFacilityRepository : IPublicFacilityRepository
    {
        private readonly AppDbContext _context;
        public PublicFacilityRepository(AppDbContext context) => _context = context;

        public async Task<PublicFacility?> GetByIdAsync(int id) => await _context.PublicFacilities
            .Include(f => f.Bookings.Where(b => b.Status == "Confirmed" || b.Status == "Pending"))
            .Include(f => f.Reviews.Where(r => r.IsApproved))
            .FirstOrDefaultAsync(f => f.Id == id);
        
        public async Task<IEnumerable<PublicFacility>> GetAllAsync() => await _context.PublicFacilities
            .Include(f => f.Reviews.Where(r => r.IsApproved))
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.Name)
            .ToListAsync();
        
        public async Task<IEnumerable<PublicFacility>> GetVisibleOnHomepageAsync() => await _context.PublicFacilities
            .Include(f => f.Reviews.Where(r => r.IsApproved))
            .Where(f => f.IsVisibleOnHomepage && f.Status == "Active")
            .OrderBy(f => f.DisplayOrder)
            .ThenByDescending(f => f.IsFeatured)
            .ThenBy(f => f.Name)
            .ToListAsync();
        
        public async Task<IEnumerable<PublicFacility>> GetByBuildingIdAsync(int? buildingId) => await _context.PublicFacilities
            .Include(f => f.Reviews.Where(r => r.IsApproved))
            .Where(f => f.BuildingId == buildingId)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync();
        
        public async Task<IEnumerable<PublicFacility>> GetByCategoryAsync(string category) => await _context.PublicFacilities
            .Include(f => f.Reviews.Where(r => r.IsApproved))
            .Where(f => f.Category == category && f.Status == "Active")
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync();
        
        public async Task AddAsync(PublicFacility facility) { await _context.PublicFacilities.AddAsync(facility); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(PublicFacility facility) { _context.PublicFacilities.Update(facility); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(PublicFacility facility) { _context.PublicFacilities.Remove(facility); await _context.SaveChangesAsync(); }
    }

    public class FacilityBookingRepository : IFacilityBookingRepository
    {
        private readonly AppDbContext _context;
        public FacilityBookingRepository(AppDbContext context) => _context = context;

        public async Task<FacilityBooking?> GetByIdAsync(int id) => await _context.FacilityBookings
            .Include(b => b.Facility)
            .FirstOrDefaultAsync(b => b.Id == id);
        
        public async Task<IEnumerable<FacilityBooking>> GetAllAsync() => await _context.FacilityBookings
            .Include(b => b.Facility)
            .OrderByDescending(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToListAsync();
        
        public async Task<IEnumerable<FacilityBooking>> GetByFacilityIdAsync(int facilityId) => await _context.FacilityBookings
            .Where(b => b.FacilityId == facilityId)
            .OrderByDescending(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToListAsync();
        
        public async Task<IEnumerable<FacilityBooking>> GetByStudentIdAsync(int studentId) => await _context.FacilityBookings
            .Include(b => b.Facility)
            .Where(b => b.StudentId == studentId)
            .OrderByDescending(b => b.BookingDate)
            .ToListAsync();
        
        public async Task<IEnumerable<FacilityBooking>> GetByDateRangeAsync(DateOnly from, DateOnly to) => await _context.FacilityBookings
            .Include(b => b.Facility)
            .Where(b => b.BookingDate >= from && b.BookingDate <= to)
            .OrderBy(b => b.BookingDate)
            .ThenBy(b => b.StartTime)
            .ToListAsync();
        
        public async Task AddAsync(FacilityBooking booking) { await _context.FacilityBookings.AddAsync(booking); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(FacilityBooking booking) { _context.FacilityBookings.Update(booking); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(FacilityBooking booking) { _context.FacilityBookings.Remove(booking); await _context.SaveChangesAsync(); }
    }

    public class FacilityReviewRepository : IFacilityReviewRepository
    {
        private readonly AppDbContext _context;
        public FacilityReviewRepository(AppDbContext context) => _context = context;

        public async Task<FacilityReview?> GetByIdAsync(int id) => await _context.FacilityReviews
            .Include(r => r.Facility)
            .FirstOrDefaultAsync(r => r.Id == id);
        
        public async Task<IEnumerable<FacilityReview>> GetAllAsync() => await _context.FacilityReviews
            .Include(r => r.Facility)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        
        public async Task<IEnumerable<FacilityReview>> GetByFacilityIdAsync(int facilityId) => await _context.FacilityReviews
            .Where(r => r.FacilityId == facilityId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        
        public async Task<IEnumerable<FacilityReview>> GetApprovedByFacilityIdAsync(int facilityId) => await _context.FacilityReviews
            .Where(r => r.FacilityId == facilityId && r.IsApproved)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
        
        public async Task AddAsync(FacilityReview review) { await _context.FacilityReviews.AddAsync(review); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(FacilityReview review) { _context.FacilityReviews.Update(review); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(FacilityReview review) { _context.FacilityReviews.Remove(review); await _context.SaveChangesAsync(); }
    }

    public class SharedUtilityRepository : ISharedUtilityRepository
    {
        private readonly AppDbContext _context;
        public SharedUtilityRepository(AppDbContext context) => _context = context;

        public async Task<SharedUtility?> GetByIdAsync(int id) => await _context.SharedUtilities
            .Include(u => u.Building)
            .Include(u => u.Events.OrderByDescending(e => e.EventDate).Take(5))
            .Include(u => u.UsageLogs.OrderByDescending(l => l.StartTime).Take(10))
            .FirstOrDefaultAsync(u => u.Id == id);
        
        public async Task<IEnumerable<SharedUtility>> GetAllAsync() => await _context.SharedUtilities
            .Include(u => u.Building)
            .OrderBy(u => u.BuildingId)
            .ThenBy(u => u.Category)
            .ThenBy(u => u.Name)
            .ToListAsync();
        
        public async Task<IEnumerable<SharedUtility>> GetByBuildingIdAsync(int buildingId) => await _context.SharedUtilities
            .Where(u => u.BuildingId == buildingId)
            .OrderBy(u => u.Category)
            .ThenBy(u => u.Name)
            .ToListAsync();
        
        public async Task<IEnumerable<SharedUtility>> GetByCategoryAsync(string category) => await _context.SharedUtilities
            .Include(u => u.Building)
            .Where(u => u.Category == category)
            .OrderBy(u => u.BuildingId)
            .ThenBy(u => u.Name)
            .ToListAsync();
        
        public async Task<IEnumerable<SharedUtility>> GetByStatusAsync(string status) => await _context.SharedUtilities
            .Include(u => u.Building)
            .Where(u => u.Status == status)
            .OrderBy(u => u.BuildingId)
            .ThenBy(u => u.Name)
            .ToListAsync();
        
        public async Task<SharedUtility?> GetByUtilityIdAsync(string utilityId) => await _context.SharedUtilities
            .Include(u => u.Building)
            .FirstOrDefaultAsync(u => u.UtilityId == utilityId);
        
        public async Task AddAsync(SharedUtility utility) { await _context.SharedUtilities.AddAsync(utility); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(SharedUtility utility) { _context.SharedUtilities.Update(utility); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(SharedUtility utility) { _context.SharedUtilities.Remove(utility); await _context.SaveChangesAsync(); }
    }

    public class UtilityEventRepository : IUtilityEventRepository
    {
        private readonly AppDbContext _context;
        public UtilityEventRepository(AppDbContext context) => _context = context;

        public async Task<UtilityEvent?> GetByIdAsync(int id) => await _context.UtilityEvents
            .Include(e => e.SharedUtility)
            .FirstOrDefaultAsync(e => e.Id == id);
        
        public async Task<IEnumerable<UtilityEvent>> GetAllAsync() => await _context.UtilityEvents
            .Include(e => e.SharedUtility)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
        
        public async Task<IEnumerable<UtilityEvent>> GetBySharedUtilityIdAsync(int sharedUtilityId) => await _context.UtilityEvents
            .Where(e => e.SharedUtilityId == sharedUtilityId)
            .OrderByDescending(e => e.EventDate)
            .ToListAsync();
        
        public async Task<IEnumerable<UtilityEvent>> GetByStatusAsync(string status) => await _context.UtilityEvents
            .Include(e => e.SharedUtility)
            .Where(e => e.Status == status)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
        
        public async Task<IEnumerable<UtilityEvent>> GetByDateRangeAsync(DateTime from, DateTime to) => await _context.UtilityEvents
            .Include(e => e.SharedUtility)
            .Where(e => e.EventDate >= from && e.EventDate <= to)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
        
        public async Task<IEnumerable<UtilityEvent>> GetUpcomingMaintenanceAsync() => await _context.UtilityEvents
            .Include(e => e.SharedUtility)
            .Where(e => (e.Status == "Scheduled" || e.Status == "InProgress") && e.EventDate >= DateTime.UtcNow)
            .OrderBy(e => e.EventDate)
            .ToListAsync();
        
        public async Task AddAsync(UtilityEvent utilityEvent) { await _context.UtilityEvents.AddAsync(utilityEvent); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(UtilityEvent utilityEvent) { _context.UtilityEvents.Update(utilityEvent); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(UtilityEvent utilityEvent) { _context.UtilityEvents.Remove(utilityEvent); await _context.SaveChangesAsync(); }
    }

    public class UtilityUsageLogRepository : IUtilityUsageLogRepository
    {
        private readonly AppDbContext _context;
        public UtilityUsageLogRepository(AppDbContext context) => _context = context;

        public async Task<UtilityUsageLog?> GetByIdAsync(int id) => await _context.UtilityUsageLogs
            .Include(l => l.SharedUtility)
            .FirstOrDefaultAsync(l => l.Id == id);
        
        public async Task<IEnumerable<UtilityUsageLog>> GetAllAsync() => await _context.UtilityUsageLogs
            .Include(l => l.SharedUtility)
            .OrderByDescending(l => l.StartTime)
            .ToListAsync();
        
        public async Task<IEnumerable<UtilityUsageLog>> GetBySharedUtilityIdAsync(int sharedUtilityId) => await _context.UtilityUsageLogs
            .Where(l => l.SharedUtilityId == sharedUtilityId)
            .OrderByDescending(l => l.StartTime)
            .ToListAsync();
        
        public async Task<IEnumerable<UtilityUsageLog>> GetByStudentIdAsync(int studentId) => await _context.UtilityUsageLogs
            .Include(l => l.SharedUtility)
            .Where(l => l.StudentId == studentId)
            .OrderByDescending(l => l.StartTime)
            .ToListAsync();
        
        public async Task<IEnumerable<UtilityUsageLog>> GetUnpaidAsync() => await _context.UtilityUsageLogs
            .Include(l => l.SharedUtility)
            .Where(l => !l.IsPaid && l.FeeCharged != null && l.FeeCharged > 0)
            .OrderBy(l => l.StartTime)
            .ToListAsync();
        
        public async Task<IEnumerable<UtilityUsageLog>> GetByDateRangeAsync(DateTime from, DateTime to) => await _context.UtilityUsageLogs
            .Include(l => l.SharedUtility)
            .Where(l => l.StartTime >= from && l.StartTime <= to)
            .OrderBy(l => l.StartTime)
            .ToListAsync();
        
        public async Task AddAsync(UtilityUsageLog log) { await _context.UtilityUsageLogs.AddAsync(log); await _context.SaveChangesAsync(); }
        public async Task UpdateAsync(UtilityUsageLog log) { _context.UtilityUsageLogs.Update(log); await _context.SaveChangesAsync(); }
        public async Task DeleteAsync(UtilityUsageLog log) { _context.UtilityUsageLogs.Remove(log); await _context.SaveChangesAsync(); }
    }
}
