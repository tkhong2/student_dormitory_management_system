using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTypesController : ControllerBase
    {
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomTypeAmenityRepository _roomTypeAmenityRepository;
        private readonly IAmenityRepository _amenityRepository;

        public RoomTypesController(
            IRoomTypeRepository roomTypeRepository,
            IRoomRepository roomRepository,
            IBuildingRepository buildingRepository,
            IRoomTypeAmenityRepository roomTypeAmenityRepository,
            IAmenityRepository amenityRepository)
        {
            _roomTypeRepository = roomTypeRepository;
            _roomRepository = roomRepository;
            _buildingRepository = buildingRepository;
            _roomTypeAmenityRepository = roomTypeAmenityRepository;
            _amenityRepository = amenityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeDto>>> GetAll()
        {
            var roomTypes = await _roomTypeRepository.GetAllAsync();
            var roomTypeDtos = new List<RoomTypeDto>();

            foreach (var rt in roomTypes)
            {
                var amenities = await _roomTypeAmenityRepository.GetByRoomTypeIdAsync(rt.Id);
                var amenityIds = amenities.Select(a => a.AmenityId).ToList();

                roomTypeDtos.Add(new RoomTypeDto
                {
                    Id = rt.Id,
                    BuildingId = rt.BuildingId,
                    Code = rt.Code,
                    Name = rt.Name,
                    Capacity = rt.Capacity,
                    PricePerMonth = rt.PricePerMonth,
                    DepositAmount = rt.DepositAmount,
                    ElectricityRate = rt.ElectricityRate,
                    WaterRate = rt.WaterRate,
                    Area = rt.Area,
                    BedType = rt.BedType,
                    HasAirConditioner = rt.HasAirConditioner,
                    HasWaterHeater = rt.HasWaterHeater,
                    HasPrivateBathroom = rt.HasPrivateBathroom,
                    HasWindowView = rt.HasWindowView,
                    Description = rt.Description,
                    ThumbnailUrl = rt.ThumbnailUrl,
                    IsActive = rt.IsActive,
                    AmenityIds = amenityIds
                });
            }

            return Ok(roomTypeDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTypeDto>> GetById(int id)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
                return NotFound(new { message = "Không tìm thấy loại phòng" });

            var amenities = await _roomTypeAmenityRepository.GetByRoomTypeIdAsync(id);
            var amenityIds = amenities.Select(a => a.AmenityId).ToList();

            var roomTypeDto = new RoomTypeDto
            {
                Id = roomType.Id,
                BuildingId = roomType.BuildingId,
                Code = roomType.Code,
                Name = roomType.Name,
                Capacity = roomType.Capacity,
                PricePerMonth = roomType.PricePerMonth,
                DepositAmount = roomType.DepositAmount,
                ElectricityRate = roomType.ElectricityRate,
                WaterRate = roomType.WaterRate,
                Area = roomType.Area,
                BedType = roomType.BedType,
                HasAirConditioner = roomType.HasAirConditioner,
                HasWaterHeater = roomType.HasWaterHeater,
                HasPrivateBathroom = roomType.HasPrivateBathroom,
                HasWindowView = roomType.HasWindowView,
                Description = roomType.Description,
                ThumbnailUrl = roomType.ThumbnailUrl,
                IsActive = roomType.IsActive,
                AmenityIds = amenityIds
            };

            return Ok(roomTypeDto);
        }

        [HttpGet("{id}/amenities")]
        public async Task<ActionResult<IEnumerable<AmenityWithQuantityDto>>> GetRoomTypeAmenities(int id)
        {
            try
            {
                var roomType = await _roomTypeRepository.GetByIdAsync(id);
                if (roomType == null)
                    return NotFound(new { message = "Không tìm thấy loại phòng" });

                var roomTypeAmenities = await _roomTypeAmenityRepository.GetByRoomTypeIdAsync(id);
                var amenityDtos = new List<AmenityWithQuantityDto>();

                foreach (var rta in roomTypeAmenities)
                {
                    var amenity = await _amenityRepository.GetByIdAsync(rta.AmenityId);
                    if (amenity != null)
                    {
                        amenityDtos.Add(new AmenityWithQuantityDto
                        {
                            Id = amenity.Id,
                            Name = amenity.Name,
                            Category = amenity.Category,
                            IconUrl = amenity.IconUrl,
                            Quantity = rta.Quantity,
                            Note = rta.Note
                        });
                    }
                }

                return Ok(amenityDtos);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting amenities for room type {id}: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return StatusCode(500, new { message = "Lỗi khi lấy danh sách tiện nghi", error = ex.Message });
            }
        }

        [HttpPost]
        public async Task<ActionResult<RoomTypeDto>> Create([FromBody] CreateRoomTypeDto dto)
        {
            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
                return BadRequest(new { message = "Tòa nhà không tồn tại" });

            var roomType = new RoomType
            {
                BuildingId = dto.BuildingId,
                Code = dto.Code,
                Name = dto.Name,
                Capacity = dto.Capacity,
                PricePerMonth = dto.PricePerMonth,
                DepositAmount = dto.DepositAmount,
                ElectricityRate = dto.ElectricityRate,
                WaterRate = dto.WaterRate,
                Area = dto.Area,
                BedType = dto.BedType,
                HasAirConditioner = dto.HasAirConditioner,
                HasWaterHeater = dto.HasWaterHeater,
                HasPrivateBathroom = dto.HasPrivateBathroom,
                HasWindowView = dto.HasWindowView,
                Description = dto.Description,
                ThumbnailUrl = dto.ThumbnailUrl,
                IsActive = dto.IsActive
            };

            await _roomTypeRepository.AddAsync(roomType);

            // Lưu amenities vào RoomTypeAmenity table
            if (dto.AmenityIds != null && dto.AmenityIds.Any())
            {
                foreach (var amenityId in dto.AmenityIds)
                {
                    var roomTypeAmenity = new RoomTypeAmenity
                    {
                        RoomTypeId = roomType.Id,
                        AmenityId = amenityId,
                        Quantity = 1
                    };
                    await _roomTypeAmenityRepository.AddAsync(roomTypeAmenity);
                }
            }

            var roomTypeDto = new RoomTypeDto
            {
                Id = roomType.Id,
                BuildingId = roomType.BuildingId,
                Code = roomType.Code,
                Name = roomType.Name,
                Capacity = roomType.Capacity,
                PricePerMonth = roomType.PricePerMonth,
                DepositAmount = roomType.DepositAmount,
                ElectricityRate = roomType.ElectricityRate,
                WaterRate = roomType.WaterRate,
                Area = roomType.Area,
                BedType = roomType.BedType,
                HasAirConditioner = roomType.HasAirConditioner,
                HasWaterHeater = roomType.HasWaterHeater,
                HasPrivateBathroom = roomType.HasPrivateBathroom,
                HasWindowView = roomType.HasWindowView,
                Description = roomType.Description,
                ThumbnailUrl = roomType.ThumbnailUrl,
                IsActive = roomType.IsActive,
                AmenityIds = dto.AmenityIds
            };

            return CreatedAtAction(nameof(GetById), new { id = roomType.Id }, roomTypeDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateRoomTypeDto dto)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
                return NotFound(new { message = "Không tìm thấy loại phòng" });

            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
                return BadRequest(new { message = "Tòa nhà không tồn tại" });

            roomType.BuildingId = dto.BuildingId;
            roomType.Code = dto.Code;
            roomType.Name = dto.Name;
            roomType.Capacity = dto.Capacity;
            roomType.PricePerMonth = dto.PricePerMonth;
            roomType.DepositAmount = dto.DepositAmount;
            roomType.ElectricityRate = dto.ElectricityRate;
            roomType.WaterRate = dto.WaterRate;
            roomType.Area = dto.Area;
            roomType.BedType = dto.BedType;
            roomType.HasAirConditioner = dto.HasAirConditioner;
            roomType.HasWaterHeater = dto.HasWaterHeater;
            roomType.HasPrivateBathroom = dto.HasPrivateBathroom;
            roomType.HasWindowView = dto.HasWindowView;
            roomType.Description = dto.Description;
            roomType.ThumbnailUrl = dto.ThumbnailUrl;
            roomType.IsActive = dto.IsActive;

            await _roomTypeRepository.UpdateAsync(roomType);

            // Xóa tất cả amenities cũ
            var existingAmenities = await _roomTypeAmenityRepository.GetByRoomTypeIdAsync(id);
            foreach (var amenity in existingAmenities)
            {
                await _roomTypeAmenityRepository.DeleteAsync(amenity);
            }

            // Thêm amenities mới
            if (dto.AmenityIds != null && dto.AmenityIds.Any())
            {
                foreach (var amenityId in dto.AmenityIds)
                {
                    var roomTypeAmenity = new RoomTypeAmenity
                    {
                        RoomTypeId = roomType.Id,
                        AmenityId = amenityId,
                        Quantity = 1
                    };
                    await _roomTypeAmenityRepository.AddAsync(roomTypeAmenity);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
                return NotFound(new { message = "Không tìm thấy loại phòng" });

            var rooms = await _roomRepository.GetAllAsync();
            if (rooms.Any(r => r.RoomTypeId == id))
                return Conflict(new { message = "Không thể xóa loại phòng vì vẫn còn phòng đang sử dụng" });

            await _roomTypeRepository.DeleteAsync(roomType);

            return NoContent();
        }
    }

    public class CreateRoomTypeDto
    {
        public int BuildingId { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public decimal PricePerMonth { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal ElectricityRate { get; set; }
        public decimal WaterRate { get; set; }
        public decimal Area { get; set; }
        public string BedType { get; set; } = "Single";
        public bool HasAirConditioner { get; set; } = false;
        public bool HasWaterHeater { get; set; } = false;
        public bool HasPrivateBathroom { get; set; } = false;
        public bool HasWindowView { get; set; } = false;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public List<int> AmenityIds { get; set; } = new List<int>();
    }

    public class AmenityWithQuantityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string? IconUrl { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
    }
}
