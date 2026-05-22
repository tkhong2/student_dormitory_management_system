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
        }
    }
}
