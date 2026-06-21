using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.Application.Interfaces
{
    public interface IBuildingRepository
    {
        Task<Building?> GetByIdAsync(int id);
        Task<IEnumerable<Building>> GetAllAsync();
        Task AddAsync(Building building);
        Task UpdateAsync(Building building);
        Task DeleteAsync(Building building);
    }

    public interface IRoomTypeRepository
    {
        Task<RoomType?> GetByIdAsync(int id);
        Task<IEnumerable<RoomType>> GetAllAsync();
        Task AddAsync(RoomType roomType);
        Task UpdateAsync(RoomType roomType);
        Task DeleteAsync(RoomType roomType);
    }

    public interface IRoomRepository
    {
        Task<Room?> GetByIdAsync(int id);
        Task<IEnumerable<Room>> GetAllAsync();
        Task<IEnumerable<Room>> GetByBuildingIdAsync(int buildingId);
        Task AddAsync(Room room);
        Task UpdateAsync(Room room);
        Task DeleteAsync(Room room);
    }

    public interface IFloorRepository
    {
        Task<Floor?> GetByIdAsync(int id);
        Task<IEnumerable<Floor>> GetAllAsync();
        Task<IEnumerable<Floor>> GetByBuildingIdAsync(int buildingId);
        Task AddAsync(Floor floor);
        Task UpdateAsync(Floor floor);
        Task DeleteAsync(Floor floor);
    }

    public interface IAmenityRepository
    {
        Task<Amenity?> GetByIdAsync(int id);
        Task<IEnumerable<Amenity>> GetAllAsync();
        Task<IEnumerable<Amenity>> GetActivesAsync();
        Task AddAsync(Amenity amenity);
        Task UpdateAsync(Amenity amenity);
        Task DeleteAsync(Amenity amenity);
    }

    public interface IBuildingAnnouncementRepository
    {
        Task<BuildingAnnouncement?> GetByIdAsync(int id);
        Task<IEnumerable<BuildingAnnouncement>> GetAllAsync();
        Task<IEnumerable<BuildingAnnouncement>> GetByBuildingIdAsync(int? buildingId);
        Task<IEnumerable<BuildingAnnouncement>> GetPublishedAsync();
        Task AddAsync(BuildingAnnouncement announcement);
        Task UpdateAsync(BuildingAnnouncement announcement);
        Task DeleteAsync(BuildingAnnouncement announcement);
    }

    public interface IRoomImageRepository
    {
        Task<RoomImage?> GetByIdAsync(int id);
        Task<IEnumerable<RoomImage>> GetByRoomIdAsync(int roomId);
        Task AddAsync(RoomImage roomImage);
        Task UpdateAsync(RoomImage roomImage);
        Task DeleteAsync(RoomImage roomImage);
    }

    public interface IRoomInspectionRepository
    {
        Task<RoomInspection?> GetByIdAsync(int id);
        Task<IEnumerable<RoomInspection>> GetAllAsync();
        Task<IEnumerable<RoomInspection>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<RoomInspection>> GetPendingApprovalsAsync();
        Task AddAsync(RoomInspection inspection);
        Task UpdateAsync(RoomInspection inspection);
        Task DeleteAsync(RoomInspection inspection);
    }

    public interface IRoomReservationRepository
    {
        Task<RoomReservation?> GetByIdAsync(int id);
        Task<IEnumerable<RoomReservation>> GetAllAsync();
        Task<IEnumerable<RoomReservation>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<RoomReservation>> GetActiveReservationsAsync();
        Task<RoomReservation?> GetByApplicationIdAsync(int applicationId);
        Task AddAsync(RoomReservation reservation);
        Task UpdateAsync(RoomReservation reservation);
        Task DeleteAsync(RoomReservation reservation);
    }

    public interface IRoomStatusLogRepository
    {
        Task<RoomStatusLog?> GetByIdAsync(int id);
        Task<IEnumerable<RoomStatusLog>> GetByRoomIdAsync(int roomId);
        Task AddAsync(RoomStatusLog log);
    }

    public interface IRoomTypeAmenityRepository
    {
        Task<IEnumerable<RoomTypeAmenity>> GetAllAsync();
        Task<RoomTypeAmenity?> GetByIdAsync(int id);
        Task<IEnumerable<RoomTypeAmenity>> GetByRoomTypeIdAsync(int roomTypeId);
        Task<IEnumerable<RoomTypeAmenity>> GetByAmenityIdAsync(int amenityId);
        Task AddAsync(RoomTypeAmenity roomTypeAmenity);
        Task UpdateAsync(RoomTypeAmenity roomTypeAmenity);
        Task DeleteAsync(RoomTypeAmenity roomTypeAmenity);
    }

    public interface IRoomAmenityInspectionRepository
    {
        Task<RoomAmenityInspection?> GetByIdAsync(int id);
        Task<IEnumerable<RoomAmenityInspection>> GetAllAsync();
        Task<IEnumerable<RoomAmenityInspection>> GetByRoomInspectionIdAsync(int roomInspectionId);
        Task<IEnumerable<RoomAmenityInspection>> GetByRoomIdAsync(int roomId);
        Task<IEnumerable<RoomAmenityInspection>> GetNeedMaintenanceAsync();
        Task AddAsync(RoomAmenityInspection inspection);
        Task UpdateAsync(RoomAmenityInspection inspection);
        Task DeleteAsync(RoomAmenityInspection inspection);
    }

    public interface IPublicFacilityRepository
    {
        Task<PublicFacility?> GetByIdAsync(int id);
        Task<IEnumerable<PublicFacility>> GetAllAsync();
        Task<IEnumerable<PublicFacility>> GetVisibleOnHomepageAsync();
        Task<IEnumerable<PublicFacility>> GetByBuildingIdAsync(int? buildingId);
        Task<IEnumerable<PublicFacility>> GetByCategoryAsync(string category);
        Task AddAsync(PublicFacility facility);
        Task UpdateAsync(PublicFacility facility);
        Task DeleteAsync(PublicFacility facility);
    }

    public interface IFacilityBookingRepository
    {
        Task<FacilityBooking?> GetByIdAsync(int id);
        Task<IEnumerable<FacilityBooking>> GetAllAsync();
        Task<IEnumerable<FacilityBooking>> GetByFacilityIdAsync(int facilityId);
        Task<IEnumerable<FacilityBooking>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<FacilityBooking>> GetByDateRangeAsync(DateOnly from, DateOnly to);
        Task AddAsync(FacilityBooking booking);
        Task UpdateAsync(FacilityBooking booking);
        Task DeleteAsync(FacilityBooking booking);
    }

    public interface IFacilityReviewRepository
    {
        Task<FacilityReview?> GetByIdAsync(int id);
        Task<IEnumerable<FacilityReview>> GetAllAsync();
        Task<IEnumerable<FacilityReview>> GetByFacilityIdAsync(int facilityId);
        Task<IEnumerable<FacilityReview>> GetApprovedByFacilityIdAsync(int facilityId);
        Task AddAsync(FacilityReview review);
        Task UpdateAsync(FacilityReview review);
        Task DeleteAsync(FacilityReview review);
    }

    public interface ISharedUtilityRepository
    {
        Task<SharedUtility?> GetByIdAsync(int id);
        Task<IEnumerable<SharedUtility>> GetAllAsync();
        Task<IEnumerable<SharedUtility>> GetByBuildingIdAsync(int buildingId);
        Task<IEnumerable<SharedUtility>> GetByCategoryAsync(string category);
        Task<IEnumerable<SharedUtility>> GetByStatusAsync(string status);
        Task<SharedUtility?> GetByUtilityIdAsync(string utilityId);
        Task AddAsync(SharedUtility utility);
        Task UpdateAsync(SharedUtility utility);
        Task DeleteAsync(SharedUtility utility);
    }

    public interface IUtilityEventRepository
    {
        Task<UtilityEvent?> GetByIdAsync(int id);
        Task<IEnumerable<UtilityEvent>> GetAllAsync();
        Task<IEnumerable<UtilityEvent>> GetBySharedUtilityIdAsync(int sharedUtilityId);
        Task<IEnumerable<UtilityEvent>> GetByStatusAsync(string status);
        Task<IEnumerable<UtilityEvent>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task<IEnumerable<UtilityEvent>> GetUpcomingMaintenanceAsync();
        Task AddAsync(UtilityEvent utilityEvent);
        Task UpdateAsync(UtilityEvent utilityEvent);
        Task DeleteAsync(UtilityEvent utilityEvent);
    }

    public interface IUtilityUsageLogRepository
    {
        Task<UtilityUsageLog?> GetByIdAsync(int id);
        Task<IEnumerable<UtilityUsageLog>> GetAllAsync();
        Task<IEnumerable<UtilityUsageLog>> GetBySharedUtilityIdAsync(int sharedUtilityId);
        Task<IEnumerable<UtilityUsageLog>> GetByStudentIdAsync(int studentId);
        Task<IEnumerable<UtilityUsageLog>> GetUnpaidAsync();
        Task<IEnumerable<UtilityUsageLog>> GetByDateRangeAsync(DateTime from, DateTime to);
        Task AddAsync(UtilityUsageLog log);
        Task UpdateAsync(UtilityUsageLog log);
        Task DeleteAsync(UtilityUsageLog log);
    }
}
