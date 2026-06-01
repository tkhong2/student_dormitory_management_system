using AutoMapper;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Building, BuildingDto>().ReverseMap();
            CreateMap<RoomType, RoomTypeDto>().ReverseMap();
            CreateMap<Room, RoomDto>().ReverseMap();
            CreateMap<Floor, FloorDto>().ReverseMap();
            CreateMap<Amenity, AmenityDto>().ReverseMap();
            
            CreateMap<BuildingAnnouncement, BuildingAnnouncementDto>()
                .ForMember(dest => dest.BuildingName, opt => opt.MapFrom(src => src.Building != null ? src.Building.Name : null))
                .ReverseMap();
            
            CreateMap<RoomImage, RoomImageDto>().ReverseMap();
            
            CreateMap<RoomInspection, RoomInspectionDto>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ReverseMap();
            
            CreateMap<RoomReservation, RoomReservationDto>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ReverseMap();
            
            CreateMap<RoomStatusLog, RoomStatusLogDto>()
                .ForMember(dest => dest.RoomNumber, opt => opt.MapFrom(src => src.Room.RoomNumber))
                .ReverseMap();
            
            CreateMap<RoomTypeAmenity, RoomTypeAmenityDto>()
                .ForMember(dest => dest.AmenityName, opt => opt.MapFrom(src => src.Amenity.Name))
                .ForMember(dest => dest.AmenityCategory, opt => opt.MapFrom(src => src.Amenity.Category))
                .ForMember(dest => dest.AmenityIconUrl, opt => opt.MapFrom(src => src.Amenity.IconUrl))
                .ReverseMap();
        }
    }
}
