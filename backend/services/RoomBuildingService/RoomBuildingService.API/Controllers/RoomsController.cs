using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IRoomImageRepository _roomImageRepository;

        public RoomsController(
            IRoomRepository roomRepository,
            IBuildingRepository buildingRepository,
            IRoomTypeRepository roomTypeRepository,
            IFloorRepository floorRepository,
            IRoomImageRepository roomImageRepository)
        {
            _roomRepository = roomRepository;
            _buildingRepository = buildingRepository;
            _roomTypeRepository = roomTypeRepository;
            _floorRepository = floorRepository;
            _roomImageRepository = roomImageRepository;
        }

        private static string? GetCoverImageUrl(Room room)
        {
            return room.Images?.FirstOrDefault(i => i.IsCoverImage)?.ImageUrl
                ?? room.Images?.FirstOrDefault()?.ImageUrl;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            var rooms = await _roomRepository.GetAllAsync();
            var roomDtos = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                FloorId = r.FloorId,
                RoomTypeId = r.RoomTypeId,
                BuildingId = r.Floor.BuildingId,
                FloorNumber = r.Floor.FloorNumber,
                BuildingName = r.Floor.Building.Name,
                RoomTypeName = r.RoomType.Name,
                Status = r.Status,
                CurrentOccupants = r.CurrentOccupants,
                MaxOccupants = r.MaxOccupants,
                Orientation = r.Orientation,
                Notes = r.Notes,
                IsLocked = r.IsLocked,
                LockReason = r.LockReason,
                QRCode = r.QRCode,
                ImageUrl = GetCoverImageUrl(r),
                LastInspectedAt = r.LastInspectedAt,
                AvailableFrom = r.AvailableFrom
            });

            return Ok(roomDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetById(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var roomDto = new RoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                FloorId = room.FloorId,
                RoomTypeId = room.RoomTypeId,
                BuildingId = room.Floor.BuildingId,
                FloorNumber = room.Floor.FloorNumber,
                BuildingName = room.Floor.Building.Name,
                RoomTypeName = room.RoomType.Name,
                Status = room.Status,
                CurrentOccupants = room.CurrentOccupants,
                MaxOccupants = room.MaxOccupants,
                Orientation = room.Orientation,
                Notes = room.Notes,
                IsLocked = room.IsLocked,
                LockReason = room.LockReason,
                QRCode = room.QRCode,
                ImageUrl = GetCoverImageUrl(room),
                LastInspectedAt = room.LastInspectedAt,
                AvailableFrom = room.AvailableFrom
            };

            return Ok(roomDto);
        }

        [HttpGet("building/{buildingId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetByBuildingId(int buildingId)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            var rooms = await _roomRepository.GetByBuildingIdAsync(buildingId);
            var roomDtos = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                FloorId = r.FloorId,
                RoomTypeId = r.RoomTypeId,
                BuildingId = r.Floor.BuildingId,
                FloorNumber = r.Floor.FloorNumber,
                BuildingName = r.Floor.Building.Name,
                RoomTypeName = r.RoomType.Name,
                Status = r.Status,
                CurrentOccupants = r.CurrentOccupants,
                MaxOccupants = r.MaxOccupants,
                Orientation = r.Orientation,
                Notes = r.Notes,
                IsLocked = r.IsLocked,
                LockReason = r.LockReason,
                QRCode = r.QRCode,
                ImageUrl = GetCoverImageUrl(r),
                LastInspectedAt = r.LastInspectedAt,
                AvailableFrom = r.AvailableFrom
            });

            return Ok(roomDtos);
        }

        [HttpPost]
        public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                return BadRequest(new { message = "Loại phòng không tồn tại" });

            var floor = await _floorRepository.GetByIdAsync(dto.FloorId);
            if (floor == null)
                return BadRequest(new { message = "Tầng không tồn tại" });

            var room = new Room
            {
                RoomNumber = dto.RoomNumber,
                FloorId = dto.FloorId,
                RoomTypeId = dto.RoomTypeId,
                Status = dto.Status,
                CurrentOccupants = dto.CurrentOccupants,
                MaxOccupants = roomType.Capacity,
                Orientation = dto.Orientation,
                Notes = dto.Notes,
                IsLocked = dto.IsLocked,
                LockReason = dto.LockReason,
                QRCode = dto.QRCode,
                LastInspectedAt = dto.LastInspectedAt,
                AvailableFrom = dto.AvailableFrom
            };

            await _roomRepository.AddAsync(room);

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                var roomImage = new RoomImage
                {
                    RoomId = room.Id,
                    ImageUrl = dto.ImageUrl,
                    IsCoverImage = true,
                    SortOrder = 0
                };
                await _roomImageRepository.AddAsync(roomImage);
            }

            var roomDto = new RoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                FloorId = room.FloorId,
                RoomTypeId = room.RoomTypeId,
                Status = room.Status,
                CurrentOccupants = room.CurrentOccupants,
                MaxOccupants = room.MaxOccupants,
                Orientation = room.Orientation,
                Notes = room.Notes,
                IsLocked = room.IsLocked,
                LockReason = room.LockReason,
                QRCode = room.QRCode,
                ImageUrl = dto.ImageUrl,
                LastInspectedAt = room.LastInspectedAt,
                AvailableFrom = room.AvailableFrom
            };

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, roomDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateRoomDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                return BadRequest(new { message = "Loại phòng không tồn tại" });

            var floor = await _floorRepository.GetByIdAsync(dto.FloorId);
            if (floor == null)
                return BadRequest(new { message = "Tầng không tồn tại" });

            room.RoomNumber = dto.RoomNumber;
            room.FloorId = dto.FloorId;
            room.RoomTypeId = dto.RoomTypeId;
            room.Status = dto.Status;
            room.CurrentOccupants = dto.CurrentOccupants;
            room.MaxOccupants = roomType.Capacity;
            room.Orientation = dto.Orientation;
            room.Notes = dto.Notes;
            room.IsLocked = dto.IsLocked;
            room.LockReason = dto.LockReason;
            room.QRCode = dto.QRCode;
            room.LastInspectedAt = dto.LastInspectedAt;
            room.AvailableFrom = dto.AvailableFrom;

            await _roomRepository.UpdateAsync(room);

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                var existingCover = room.Images.FirstOrDefault(i => i.IsCoverImage) ?? room.Images.FirstOrDefault();
                if (existingCover != null)
                {
                    existingCover.ImageUrl = dto.ImageUrl;
                    existingCover.IsCoverImage = true;
                    await _roomImageRepository.UpdateAsync(existingCover);
                }
                else
                {
                    var roomImage = new RoomImage
                    {
                        RoomId = room.Id,
                        ImageUrl = dto.ImageUrl,
                        IsCoverImage = true,
                        SortOrder = 0
                    };
                    await _roomImageRepository.AddAsync(roomImage);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
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
        public int FloorId { get; set; }
        public int RoomTypeId { get; set; }
        public string Status { get; set; } = "Available";
        public int CurrentOccupants { get; set; }
        public int MaxOccupants { get; set; }
        public string? Orientation { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsLocked { get; set; } = false;
        public string? LockReason { get; set; }
        public string? QRCode { get; set; }
        public DateTime? LastInspectedAt { get; set; }
        public DateTime? AvailableFrom { get; set; }
    }
}
