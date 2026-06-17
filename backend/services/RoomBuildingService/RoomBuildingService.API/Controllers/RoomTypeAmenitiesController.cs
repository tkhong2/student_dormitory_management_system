using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTypeAmenitiesController : ControllerBase
    {
        private readonly IRoomTypeAmenityRepository _roomTypeAmenityRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IAmenityRepository _amenityRepository;

        public RoomTypeAmenitiesController(
            IRoomTypeAmenityRepository roomTypeAmenityRepository,
            IRoomTypeRepository roomTypeRepository,
            IAmenityRepository amenityRepository)
        {
            _roomTypeAmenityRepository = roomTypeAmenityRepository;
            _roomTypeRepository = roomTypeRepository;
            _amenityRepository = amenityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeAmenityDto>>> GetAll()
        {
            var roomTypeAmenities = await _roomTypeAmenityRepository.GetAllAsync();
            var roomTypeAmenityDtos = roomTypeAmenities.Select(rta => new RoomTypeAmenityDto
            {
                Id = rta.Id,
                RoomTypeId = rta.RoomTypeId,
                AmenityId = rta.AmenityId,
                AmenityName = rta.Amenity?.Name ?? "",
                AmenityCategory = rta.Amenity?.Category ?? "",
                AmenityIconUrl = rta.Amenity?.IconUrl,
                Quantity = rta.Quantity,
                Note = rta.Note
            });

            return Ok(roomTypeAmenityDtos);
        }

        [HttpGet("roomtype/{roomTypeId}")]
        public async Task<ActionResult<IEnumerable<RoomTypeAmenityDto>>> GetByRoomTypeId(int roomTypeId)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(roomTypeId);
            if (roomType == null)
                return NotFound(new { message = "Không tìm thấy loại phòng" });

            var roomTypeAmenities = await _roomTypeAmenityRepository.GetByRoomTypeIdAsync(roomTypeId);
            var roomTypeAmenityDtos = roomTypeAmenities.Select(rta => new RoomTypeAmenityDto
            {
                Id = rta.Id,
                RoomTypeId = rta.RoomTypeId,
                AmenityId = rta.AmenityId,
                AmenityName = rta.Amenity.Name,
                AmenityCategory = rta.Amenity.Category,
                AmenityIconUrl = rta.Amenity.IconUrl,
                Quantity = rta.Quantity,
                Note = rta.Note
            });

            return Ok(roomTypeAmenityDtos);
        }

        [HttpGet("amenity/{amenityId}")]
        public async Task<ActionResult<IEnumerable<RoomTypeAmenityDto>>> GetByAmenityId(int amenityId)
        {
            var amenity = await _amenityRepository.GetByIdAsync(amenityId);
            if (amenity == null)
                return NotFound(new { message = "Không tìm thấy tiện nghi" });

            var roomTypeAmenities = await _roomTypeAmenityRepository.GetByAmenityIdAsync(amenityId);
            var roomTypeAmenityDtos = roomTypeAmenities.Select(rta => new RoomTypeAmenityDto
            {
                Id = rta.Id,
                RoomTypeId = rta.RoomTypeId,
                AmenityId = rta.AmenityId,
                AmenityName = rta.Amenity.Name,
                AmenityCategory = rta.Amenity.Category,
                AmenityIconUrl = rta.Amenity.IconUrl,
                Quantity = rta.Quantity,
                Note = rta.Note
            });

            return Ok(roomTypeAmenityDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTypeAmenityDto>> GetById(int id)
        {
            var roomTypeAmenity = await _roomTypeAmenityRepository.GetByIdAsync(id);
            if (roomTypeAmenity == null)
                return NotFound(new { message = "Không tìm thấy tiện nghi loại phòng" });

            var roomTypeAmenityDto = new RoomTypeAmenityDto
            {
                Id = roomTypeAmenity.Id,
                RoomTypeId = roomTypeAmenity.RoomTypeId,
                AmenityId = roomTypeAmenity.AmenityId,
                AmenityName = roomTypeAmenity.Amenity.Name,
                AmenityCategory = roomTypeAmenity.Amenity.Category,
                AmenityIconUrl = roomTypeAmenity.Amenity.IconUrl,
                Quantity = roomTypeAmenity.Quantity,
                Note = roomTypeAmenity.Note
            };

            return Ok(roomTypeAmenityDto);
        }

        [HttpPost]
        public async Task<ActionResult<RoomTypeAmenityDto>> Create([FromBody] CreateRoomTypeAmenityDto dto)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                return BadRequest(new { message = "Loại phòng không tồn tại" });

            var amenity = await _amenityRepository.GetByIdAsync(dto.AmenityId);
            if (amenity == null)
                return BadRequest(new { message = "Tiện nghi không tồn tại" });

            var roomTypeAmenity = new RoomTypeAmenity
            {
                RoomTypeId = dto.RoomTypeId,
                AmenityId = dto.AmenityId,
                Quantity = dto.Quantity,
                Note = dto.Note
            };

            await _roomTypeAmenityRepository.AddAsync(roomTypeAmenity);

            var roomTypeAmenityDto = new RoomTypeAmenityDto
            {
                Id = roomTypeAmenity.Id,
                RoomTypeId = roomTypeAmenity.RoomTypeId,
                AmenityId = roomTypeAmenity.AmenityId,
                AmenityName = amenity.Name,
                AmenityCategory = amenity.Category,
                AmenityIconUrl = amenity.IconUrl,
                Quantity = roomTypeAmenity.Quantity,
                Note = roomTypeAmenity.Note
            };

            return CreatedAtAction(nameof(GetById), new { id = roomTypeAmenity.Id }, roomTypeAmenityDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateRoomTypeAmenityDto dto)
        {
            var roomTypeAmenity = await _roomTypeAmenityRepository.GetByIdAsync(id);
            if (roomTypeAmenity == null)
                return NotFound(new { message = "Không tìm thấy tiện nghi loại phòng" });

            roomTypeAmenity.Quantity = dto.Quantity;
            roomTypeAmenity.Note = dto.Note;

            await _roomTypeAmenityRepository.UpdateAsync(roomTypeAmenity);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var roomTypeAmenity = await _roomTypeAmenityRepository.GetByIdAsync(id);
            if (roomTypeAmenity == null)
                return NotFound(new { message = "Không tìm thấy tiện nghi loại phòng" });

            await _roomTypeAmenityRepository.DeleteAsync(roomTypeAmenity);

            return NoContent();
        }
    }
}
