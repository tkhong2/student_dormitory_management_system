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

        public RoomTypesController(IRoomTypeRepository roomTypeRepository)
        {
            _roomTypeRepository = roomTypeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTypeDto>>> GetAll()
        {
            var roomTypes = await _roomTypeRepository.GetAllAsync();
            var roomTypeDtos = roomTypes.Select(rt => new RoomTypeDto
            {
                Id = rt.Id,
                Name = rt.Name,
                Price = rt.Price,
                Capacity = rt.Capacity,
                Description = rt.Description
            });

            return Ok(roomTypeDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTypeDto>> GetById(Guid id)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
                return NotFound(new { message = "Không tìm thấy loại phòng" });

            var roomTypeDto = new RoomTypeDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Price = roomType.Price,
                Capacity = roomType.Capacity,
                Description = roomType.Description
            };

            return Ok(roomTypeDto);
        }

        [HttpPost]
        public async Task<ActionResult<RoomTypeDto>> Create([FromBody] CreateRoomTypeDto dto)
        {
            var roomType = new RoomType
            {
                Name = dto.Name,
                Price = dto.Price,
                Capacity = dto.Capacity,
                Description = dto.Description
            };

            await _roomTypeRepository.AddAsync(roomType);

            var roomTypeDto = new RoomTypeDto
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Price = roomType.Price,
                Capacity = roomType.Capacity,
                Description = roomType.Description
            };

            return CreatedAtAction(nameof(GetById), new { id = roomType.Id }, roomTypeDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] CreateRoomTypeDto dto)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
                return NotFound(new { message = "Không tìm thấy loại phòng" });

            roomType.Name = dto.Name;
            roomType.Price = dto.Price;
            roomType.Capacity = dto.Capacity;
            roomType.Description = dto.Description;

            await _roomTypeRepository.UpdateAsync(roomType);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(id);
            if (roomType == null)
                return NotFound(new { message = "Không tìm thấy loại phòng" });

            await _roomTypeRepository.DeleteAsync(roomType);

            return NoContent();
        }
    }

    public class CreateRoomTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Capacity { get; set; }
        public string Description { get; set; } = string.Empty;
    }
}
