using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;
using RoomBuildingService.Domain.Enums;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;

        public RoomsController(
            IRoomRepository roomRepository,
            IBuildingRepository buildingRepository,
            IRoomTypeRepository roomTypeRepository)
        {
            _roomRepository = roomRepository;
            _buildingRepository = buildingRepository;
            _roomTypeRepository = roomTypeRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            var rooms = await _roomRepository.GetAllAsync();
            var roomDtos = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Floor = r.Floor,
                BuildingId = r.BuildingId,
                RoomTypeId = r.RoomTypeId,
                Status = r.Status.ToString(),
                CurrentOccupancy = r.CurrentOccupancy,
                ImageUrl = r.ImageUrl
            });

            return Ok(roomDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetById(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var roomDto = new RoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor,
                BuildingId = room.BuildingId,
                RoomTypeId = room.RoomTypeId,
                Status = room.Status.ToString(),
                CurrentOccupancy = room.CurrentOccupancy,
                ImageUrl = room.ImageUrl
            };

            return Ok(roomDto);
        }

        [HttpGet("building/{buildingId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetByBuildingId(Guid buildingId)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            var rooms = await _roomRepository.GetByBuildingIdAsync(buildingId);
            var roomDtos = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                Floor = r.Floor,
                BuildingId = r.BuildingId,
                RoomTypeId = r.RoomTypeId,
                Status = r.Status.ToString(),
                CurrentOccupancy = r.CurrentOccupancy,
                ImageUrl = r.ImageUrl
            });

            return Ok(roomDtos);
        }

        [HttpPost]
        public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
        {
            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
                return BadRequest(new { message = "Tòa nhà không tồn tại" });

            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                return BadRequest(new { message = "Loại phòng không tồn tại" });

            if (!Enum.TryParse<RoomStatus>(dto.Status, out var roomStatus))
                return BadRequest(new { message = "Trạng thái phòng không hợp lệ" });

            var room = new Room
            {
                RoomNumber = dto.RoomNumber,
                Floor = dto.Floor,
                BuildingId = dto.BuildingId,
                RoomTypeId = dto.RoomTypeId,
                Status = roomStatus,
                CurrentOccupancy = dto.CurrentOccupancy,
                ImageUrl = dto.ImageUrl
            };

            await _roomRepository.AddAsync(room);

            var roomDto = new RoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                Floor = room.Floor,
                BuildingId = room.BuildingId,
                RoomTypeId = room.RoomTypeId,
                Status = room.Status.ToString(),
                CurrentOccupancy = room.CurrentOccupancy,
                ImageUrl = room.ImageUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, roomDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] CreateRoomDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
                return BadRequest(new { message = "Tòa nhà không tồn tại" });

            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                return BadRequest(new { message = "Loại phòng không tồn tại" });

            if (!Enum.TryParse<RoomStatus>(dto.Status, out var roomStatus))
                return BadRequest(new { message = "Trạng thái phòng không hợp lệ" });

            room.RoomNumber = dto.RoomNumber;
            room.Floor = dto.Floor;
            room.BuildingId = dto.BuildingId;
            room.RoomTypeId = dto.RoomTypeId;
            room.Status = roomStatus;
            room.CurrentOccupancy = dto.CurrentOccupancy;
            room.ImageUrl = dto.ImageUrl;

            await _roomRepository.UpdateAsync(room);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            await _roomRepository.DeleteAsync(room);

            return NoContent();
        }
    }

    public class CreateRoomDto
    {
        public string RoomNumber { get; set; } = string.Empty;
        public int Floor { get; set; }
        public Guid BuildingId { get; set; }
        public Guid RoomTypeId { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CurrentOccupancy { get; set; }
        public string? ImageUrl { get; set; }
    }
}
