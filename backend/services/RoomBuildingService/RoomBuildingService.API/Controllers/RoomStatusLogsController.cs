using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomStatusLogsController : ControllerBase
    {
        private readonly IRoomStatusLogRepository _statusLogRepository;
        private readonly IRoomRepository _roomRepository;

        public RoomStatusLogsController(
            IRoomStatusLogRepository statusLogRepository,
            IRoomRepository roomRepository)
        {
            _statusLogRepository = statusLogRepository;
            _roomRepository = roomRepository;
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<RoomStatusLogDto>>> GetByRoomId(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var logs = await _statusLogRepository.GetByRoomIdAsync(roomId);
            var logDtos = logs.Select(l => new RoomStatusLogDto
            {
                Id = l.Id,
                RoomId = l.RoomId,
                RoomNumber = l.Room.RoomNumber,
                OldStatus = l.OldStatus,
                NewStatus = l.NewStatus,
                Reason = l.Reason,
                ChangedByUserId = l.ChangedByUserId,
                ChangedByName = l.ChangedByName,
                ChangedAt = l.ChangedAt
            });

            return Ok(logDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomStatusLogDto>> GetById(int id)
        {
            var log = await _statusLogRepository.GetByIdAsync(id);
            if (log == null)
                return NotFound(new { message = "Không tìm thấy log" });

            var logDto = new RoomStatusLogDto
            {
                Id = log.Id,
                RoomId = log.RoomId,
                RoomNumber = log.Room.RoomNumber,
                OldStatus = log.OldStatus,
                NewStatus = log.NewStatus,
                Reason = log.Reason,
                ChangedByUserId = log.ChangedByUserId,
                ChangedByName = log.ChangedByName,
                ChangedAt = log.ChangedAt
            };

            return Ok(logDto);
        }

        [HttpPost]
        public async Task<ActionResult<RoomStatusLogDto>> Create([FromBody] CreateRoomStatusLogDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null)
                return BadRequest(new { message = "Phòng không tồn tại" });

            var log = new RoomStatusLog
            {
                RoomId = dto.RoomId,
                OldStatus = dto.OldStatus,
                NewStatus = dto.NewStatus,
                Reason = dto.Reason,
                ChangedByUserId = dto.ChangedByUserId,
                ChangedByName = dto.ChangedByName,
                ChangedAt = DateTime.UtcNow
            };

            await _statusLogRepository.AddAsync(log);

            var logDto = new RoomStatusLogDto
            {
                Id = log.Id,
                RoomId = log.RoomId,
                OldStatus = log.OldStatus,
                NewStatus = log.NewStatus,
                Reason = log.Reason,
                ChangedByUserId = log.ChangedByUserId,
                ChangedByName = log.ChangedByName,
                ChangedAt = log.ChangedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = log.Id }, logDto);
        }
    }

    public class CreateRoomStatusLogDto
    {
        public int RoomId { get; set; }
        public string OldStatus { get; set; } = string.Empty;
        public string NewStatus { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public int ChangedByUserId { get; set; }
        public string ChangedByName { get; set; } = string.Empty;
    }
}
