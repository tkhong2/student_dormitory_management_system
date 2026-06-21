using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacilityBookingsController : ControllerBase
    {
        private readonly IFacilityBookingRepository _bookingRepository;
        private readonly IPublicFacilityRepository _facilityRepository;

        public FacilityBookingsController(
            IFacilityBookingRepository bookingRepository,
            IPublicFacilityRepository facilityRepository)
        {
            _bookingRepository = bookingRepository;
            _facilityRepository = facilityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityBookingDto>>> GetAll()
        {
            var bookings = await _bookingRepository.GetAllAsync();
            return Ok(bookings.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityBookingDto>> GetById(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Không tìm thấy đặt lịch" });

            return Ok(MapToDto(booking));
        }

        [HttpGet("facility/{facilityId}")]
        public async Task<ActionResult<IEnumerable<FacilityBookingDto>>> GetByFacilityId(int facilityId)
        {
            var bookings = await _bookingRepository.GetByFacilityIdAsync(facilityId);
            return Ok(bookings.Select(MapToDto));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<FacilityBookingDto>>> GetByStudentId(int studentId)
        {
            var bookings = await _bookingRepository.GetByStudentIdAsync(studentId);
            return Ok(bookings.Select(MapToDto));
        }

        [HttpGet("date-range")]
        public async Task<ActionResult<IEnumerable<FacilityBookingDto>>> GetByDateRange([FromQuery] string from, [FromQuery] string to)
        {
            if (!DateOnly.TryParse(from, out var fromDate) || !DateOnly.TryParse(to, out var toDate))
                return BadRequest(new { message = "Định dạng ngày không hợp lệ" });

            var bookings = await _bookingRepository.GetByDateRangeAsync(fromDate, toDate);
            return Ok(bookings.Select(MapToDto));
        }

        [HttpPost]
        public async Task<ActionResult<FacilityBookingDto>> Create([FromBody] CreateFacilityBookingDto dto)
        {
            var facility = await _facilityRepository.GetByIdAsync(dto.FacilityId);
            if (facility == null)
                return BadRequest(new { message = "Tiện ích không tồn tại" });

            if (facility.Status != "Active")
                return BadRequest(new { message = "Tiện ích không hoạt động" });

            var booking = new FacilityBooking
            {
                FacilityId = dto.FacilityId,
                StudentId = dto.StudentId,
                StudentName = dto.StudentName,
                StudentPhone = dto.StudentPhone,
                BookingDate = dto.BookingDate,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                Purpose = dto.Purpose,
                Status = "Pending"
            };

            // Tính phí tự động
            if (facility.FeePerSession.HasValue)
            {
                booking.Fee = facility.FeePerSession;
            }
            else if (facility.FeePerHour.HasValue && dto.StartTime.HasValue && dto.EndTime.HasValue)
            {
                var hours = (dto.EndTime.Value.ToTimeSpan() - dto.StartTime.Value.ToTimeSpan()).TotalHours;
                booking.Fee = (decimal)hours * facility.FeePerHour.Value;
            }

            await _bookingRepository.AddAsync(booking);

            return CreatedAtAction(nameof(GetById), new { id = booking.Id }, MapToDto(booking));
        }

        [HttpPut("{id}/confirm")]
        public async Task<ActionResult> Confirm(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Không tìm thấy đặt lịch" });

            if (booking.Status != "Pending")
                return BadRequest(new { message = "Chỉ có thể xác nhận đặt lịch đang chờ" });

            booking.Status = "Confirmed";
            await _bookingRepository.UpdateAsync(booking);

            return NoContent();
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> Cancel(int id, [FromBody] CancelBookingDto dto)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Không tìm thấy đặt lịch" });

            if (booking.Status == "Completed" || booking.Status == "Cancelled")
                return BadRequest(new { message = "Không thể hủy đặt lịch này" });

            booking.Status = "Cancelled";
            booking.CancellationReason = dto.Reason;
            booking.CancelledAt = DateTime.UtcNow;

            await _bookingRepository.UpdateAsync(booking);

            return NoContent();
        }

        [HttpPut("{id}/complete")]
        public async Task<ActionResult> Complete(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Không tìm thấy đặt lịch" });

            if (booking.Status != "Confirmed")
                return BadRequest(new { message = "Chỉ có thể hoàn thành đặt lịch đã xác nhận" });

            booking.Status = "Completed";
            await _bookingRepository.UpdateAsync(booking);

            // Tăng số lượt sử dụng
            var facility = await _facilityRepository.GetByIdAsync(booking.FacilityId);
            if (facility != null)
            {
                facility.TotalUsageCount++;
                await _facilityRepository.UpdateAsync(facility);
            }

            return NoContent();
        }

        [HttpPut("{id}/mark-paid")]
        public async Task<ActionResult> MarkPaid(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Không tìm thấy đặt lịch" });

            booking.IsPaid = true;
            await _bookingRepository.UpdateAsync(booking);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null)
                return NotFound(new { message = "Không tìm thấy đặt lịch" });

            await _bookingRepository.DeleteAsync(booking);

            return NoContent();
        }

        private static FacilityBookingDto MapToDto(FacilityBooking booking)
        {
            return new FacilityBookingDto
            {
                Id = booking.Id,
                FacilityId = booking.FacilityId,
                FacilityName = booking.Facility?.Name ?? string.Empty,
                StudentId = booking.StudentId,
                StudentName = booking.StudentName,
                StudentPhone = booking.StudentPhone,
                BookingDate = booking.BookingDate,
                StartTime = booking.StartTime,
                EndTime = booking.EndTime,
                Status = booking.Status,
                Purpose = booking.Purpose,
                Fee = booking.Fee,
                IsPaid = booking.IsPaid,
                CancellationReason = booking.CancellationReason,
                CancelledAt = booking.CancelledAt,
                CreatedAt = booking.CreatedAt
            };
        }
    }

    public class CancelBookingDto
    {
        public string? Reason { get; set; }
    }
}
