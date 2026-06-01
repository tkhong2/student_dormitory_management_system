using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomReservationsController : ControllerBase
    {
        private readonly IRoomReservationRepository _reservationRepository;
        private readonly IRoomRepository _roomRepository;

        public RoomReservationsController(
            IRoomReservationRepository reservationRepository,
            IRoomRepository roomRepository)
        {
            _reservationRepository = reservationRepository;
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomReservationDto>>> GetAll()
        {
            var reservations = await _reservationRepository.GetAllAsync();
            var reservationDtos = reservations.Select(r => new RoomReservationDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                RoomNumber = r.Room.RoomNumber,
                ApplicationId = r.ApplicationId,
                StudentId = r.StudentId,
                StudentName = r.StudentName,
                Status = r.Status,
                ExpiresAt = r.ExpiresAt,
                Note = r.Note,
                CreatedByUserId = r.CreatedByUserId,
                CreatedByName = r.CreatedByName,
                ReleasedAt = r.ReleasedAt,
                ReleaseReason = r.ReleaseReason,
                CreatedAt = r.CreatedAt
            });

            return Ok(reservationDtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<RoomReservationDto>>> GetActive()
        {
            var reservations = await _reservationRepository.GetActiveReservationsAsync();
            var reservationDtos = reservations.Select(r => new RoomReservationDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                RoomNumber = r.Room.RoomNumber,
                ApplicationId = r.ApplicationId,
                StudentId = r.StudentId,
                StudentName = r.StudentName,
                Status = r.Status,
                ExpiresAt = r.ExpiresAt,
                Note = r.Note,
                CreatedByUserId = r.CreatedByUserId,
                CreatedByName = r.CreatedByName,
                ReleasedAt = r.ReleasedAt,
                ReleaseReason = r.ReleaseReason,
                CreatedAt = r.CreatedAt
            });

            return Ok(reservationDtos);
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<RoomReservationDto>>> GetByRoomId(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var reservations = await _reservationRepository.GetByRoomIdAsync(roomId);
            var reservationDtos = reservations.Select(r => new RoomReservationDto
            {
                Id = r.Id,
                RoomId = r.RoomId,
                RoomNumber = r.Room.RoomNumber,
                ApplicationId = r.ApplicationId,
                StudentId = r.StudentId,
                StudentName = r.StudentName,
                Status = r.Status,
                ExpiresAt = r.ExpiresAt,
                Note = r.Note,
                CreatedByUserId = r.CreatedByUserId,
                CreatedByName = r.CreatedByName,
                ReleasedAt = r.ReleasedAt,
                ReleaseReason = r.ReleaseReason,
                CreatedAt = r.CreatedAt
            });

            return Ok(reservationDtos);
        }

        [HttpGet("application/{applicationId}")]
        public async Task<ActionResult<RoomReservationDto>> GetByApplicationId(int applicationId)
        {
            var reservation = await _reservationRepository.GetByApplicationIdAsync(applicationId);
            if (reservation == null)
                return NotFound(new { message = "Không tìm thấy đặt chỗ cho đơn đăng ký này" });

            var reservationDto = new RoomReservationDto
            {
                Id = reservation.Id,
                RoomId = reservation.RoomId,
                RoomNumber = reservation.Room.RoomNumber,
                ApplicationId = reservation.ApplicationId,
                StudentId = reservation.StudentId,
                StudentName = reservation.StudentName,
                Status = reservation.Status,
                ExpiresAt = reservation.ExpiresAt,
                Note = reservation.Note,
                CreatedByUserId = reservation.CreatedByUserId,
                CreatedByName = reservation.CreatedByName,
                ReleasedAt = reservation.ReleasedAt,
                ReleaseReason = reservation.ReleaseReason,
                CreatedAt = reservation.CreatedAt
            };

            return Ok(reservationDto);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomReservationDto>> GetById(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Không tìm thấy đặt chỗ" });

            var reservationDto = new RoomReservationDto
            {
                Id = reservation.Id,
                RoomId = reservation.RoomId,
                RoomNumber = reservation.Room.RoomNumber,
                ApplicationId = reservation.ApplicationId,
                StudentId = reservation.StudentId,
                StudentName = reservation.StudentName,
                Status = reservation.Status,
                ExpiresAt = reservation.ExpiresAt,
                Note = reservation.Note,
                CreatedByUserId = reservation.CreatedByUserId,
                CreatedByName = reservation.CreatedByName,
                ReleasedAt = reservation.ReleasedAt,
                ReleaseReason = reservation.ReleaseReason,
                CreatedAt = reservation.CreatedAt
            };

            return Ok(reservationDto);
        }

        [HttpPost]
        public async Task<ActionResult<RoomReservationDto>> Create([FromBody] CreateRoomReservationDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null)
                return BadRequest(new { message = "Phòng không tồn tại" });

            // Check if room is available
            if (room.Status != "Available" && room.Status != "Reserved")
                return BadRequest(new { message = "Phòng không khả dụng để đặt chỗ" });

            // Check if there's already an active reservation for this application
            var existingReservation = await _reservationRepository.GetByApplicationIdAsync(dto.ApplicationId);
            if (existingReservation != null)
                return BadRequest(new { message = "Đơn đăng ký này đã có đặt chỗ" });

            var reservation = new RoomReservation
            {
                RoomId = dto.RoomId,
                ApplicationId = dto.ApplicationId,
                StudentId = dto.StudentId,
                StudentName = dto.StudentName,
                Status = "Holding",
                ExpiresAt = dto.ExpiresAt,
                Note = dto.Note,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedByName = dto.CreatedByName
            };

            await _reservationRepository.AddAsync(reservation);

            // Update room status to Reserved if not already
            if (room.Status == "Available")
            {
                room.Status = "Reserved";
                await _roomRepository.UpdateAsync(room);
            }

            var reservationDto = new RoomReservationDto
            {
                Id = reservation.Id,
                RoomId = reservation.RoomId,
                ApplicationId = reservation.ApplicationId,
                StudentId = reservation.StudentId,
                StudentName = reservation.StudentName,
                Status = reservation.Status,
                ExpiresAt = reservation.ExpiresAt,
                Note = reservation.Note,
                CreatedByUserId = reservation.CreatedByUserId,
                CreatedByName = reservation.CreatedByName,
                CreatedAt = reservation.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservationDto);
        }

        [HttpPost("{id}/confirm")]
        public async Task<ActionResult> Confirm(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Không tìm thấy đặt chỗ" });

            if (reservation.Status != "Holding")
                return BadRequest(new { message = "Chỉ có thể xác nhận đặt chỗ đang giữ" });

            reservation.Status = "Confirmed";
            reservation.ReleasedAt = DateTime.UtcNow;
            reservation.ReleaseReason = "Confirmed";

            await _reservationRepository.UpdateAsync(reservation);

            return NoContent();
        }

        [HttpPost("{id}/release")]
        public async Task<ActionResult> Release(int id, [FromBody] ReleaseReservationDto dto)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Không tìm thấy đặt chỗ" });

            if (reservation.Status != "Holding")
                return BadRequest(new { message = "Chỉ có thể hủy đặt chỗ đang giữ" });

            reservation.Status = "Released";
            reservation.ReleasedAt = DateTime.UtcNow;
            reservation.ReleaseReason = dto.Reason ?? "AdminCancelled";

            await _reservationRepository.UpdateAsync(reservation);

            // Check if room should be set back to Available
            var room = await _roomRepository.GetByIdAsync(reservation.RoomId);
            if (room != null && room.Status == "Reserved")
            {
                var activeReservations = await _reservationRepository.GetByRoomIdAsync(room.Id);
                var hasActiveReservation = activeReservations.Any(r => r.Status == "Holding" && r.ExpiresAt > DateTime.UtcNow);
                
                if (!hasActiveReservation && room.CurrentOccupants < room.MaxOccupants)
                {
                    room.Status = "Available";
                    await _roomRepository.UpdateAsync(room);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var reservation = await _reservationRepository.GetByIdAsync(id);
            if (reservation == null)
                return NotFound(new { message = "Không tìm thấy đặt chỗ" });

            await _reservationRepository.DeleteAsync(reservation);

            return NoContent();
        }
    }

    public class ReleaseReservationDto
    {
        public string? Reason { get; set; }
    }
}
