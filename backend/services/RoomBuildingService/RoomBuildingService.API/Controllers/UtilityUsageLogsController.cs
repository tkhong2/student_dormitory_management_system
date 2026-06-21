using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UtilityUsageLogsController : ControllerBase
    {
        private readonly IUtilityUsageLogRepository _repository;
        private readonly ISharedUtilityRepository _utilityRepository;

        public UtilityUsageLogsController(IUtilityUsageLogRepository repository, ISharedUtilityRepository utilityRepository)
        {
            _repository = repository;
            _utilityRepository = utilityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UtilityUsageLogDto>>> GetAll()
        {
            var logs = await _repository.GetAllAsync();
            return Ok(logs.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UtilityUsageLogDto>> GetById(int id)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null) return NotFound();
            return Ok(MapToDto(log));
        }

        [HttpGet("utility/{utilityId}")]
        public async Task<ActionResult<IEnumerable<UtilityUsageLogDto>>> GetByUtility(int utilityId)
        {
            var logs = await _repository.GetBySharedUtilityIdAsync(utilityId);
            return Ok(logs.Select(MapToDto));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<UtilityUsageLogDto>>> GetByStudent(int studentId)
        {
            var logs = await _repository.GetByStudentIdAsync(studentId);
            return Ok(logs.Select(MapToDto));
        }

        [HttpGet("unpaid")]
        public async Task<ActionResult<IEnumerable<UtilityUsageLogDto>>> GetUnpaid()
        {
            var logs = await _repository.GetUnpaidAsync();
            return Ok(logs.Select(MapToDto));
        }

        [HttpPost]
        public async Task<ActionResult<UtilityUsageLogDto>> Create([FromBody] CreateUtilityUsageLogDto dto)
        {
            // Validate booking time
            if (dto.EndTime.HasValue && dto.EndTime.Value <= dto.StartTime)
            {
                return BadRequest(new { message = "Thời gian kết thúc phải sau thời gian bắt đầu" });
            }

            // Check for booking conflicts
            if (dto.EndTime.HasValue)
            {
                var existingBookings = await _repository.GetBySharedUtilityIdAsync(dto.SharedUtilityId);
                var hasConflict = existingBookings.Any(b => 
                    b.EndTime.HasValue &&
                    b.Id != 0 &&
                    // Check if time ranges overlap
                    dto.StartTime < b.EndTime.Value && dto.EndTime.Value > b.StartTime
                );

                if (hasConflict)
                {
                    return BadRequest(new { message = "Tiện ích đã được đặt trong khoảng thời gian này" });
                }
            }

            var log = new UtilityUsageLog
            {
                SharedUtilityId = dto.SharedUtilityId,
                StudentId = dto.StudentId,
                StudentName = dto.StudentName,
                RoomNumber = dto.RoomNumber,
                StartTime = dto.StartTime,
                EndTime = dto.EndTime,
                FeeCharged = dto.FeeCharged,
                Notes = dto.Notes,
                IsPaid = false
            };

            await _repository.AddAsync(log);

            // Update utility usage statistics
            var utility = await _utilityRepository.GetByIdAsync(dto.SharedUtilityId);
            if (utility != null)
            {
                utility.TotalUsageCount++;
                utility.LastUsedAt = dto.StartTime;
                await _utilityRepository.UpdateAsync(utility);
            }

            // Return DTO with utility info for QR generation
            var result = MapToDto(log);
            result.UtilityName = utility?.Name;
            
            return CreatedAtAction(nameof(GetById), new { id = log.Id }, result);
        }

        [HttpPut("{id}/mark-paid")]
        public async Task<ActionResult> MarkAsPaid(int id, [FromQuery] int? invoiceId = null)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null) return NotFound();

            log.IsPaid = true;
            log.InvoiceId = invoiceId;
            await _repository.UpdateAsync(log);
            return NoContent();
        }

        [HttpPut("{id}/end-usage")]
        public async Task<ActionResult> EndUsage(int id, [FromBody] DateTime endTime)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null) return NotFound();

            log.EndTime = endTime;
            await _repository.UpdateAsync(log);

            // Update utility status back to Available
            var utility = await _utilityRepository.GetByIdAsync(log.SharedUtilityId);
            if (utility != null && utility.Status == "InUse")
            {
                utility.Status = "Available";
                await _utilityRepository.UpdateAsync(utility);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null) return NotFound();

            await _repository.DeleteAsync(log);
            return NoContent();
        }

        /// <summary>
        /// Check if utility usage has been paid via Sepay webhook
        /// This endpoint is called by BillingMaintenanceService after receiving Sepay webhook
        /// or manually by frontend to check payment status
        /// </summary>
        [HttpPost("{id}/check-payment")]
        public async Task<ActionResult> CheckPayment(int id)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null) 
                return NotFound(new { message = "Usage log not found" });

            // Return current payment status
            return Ok(new
            {
                id = log.Id,
                isPaid = log.IsPaid,
                feeCharged = log.FeeCharged,
                message = log.IsPaid ? "Đã thanh toán" : "Chưa thanh toán"
            });
        }

        /// <summary>
        /// Webhook endpoint to mark utility usage as paid (called from BillingMaintenanceService webhook)
        /// </summary>
        [HttpPost("{id}/mark-paid-from-webhook")]
        public async Task<ActionResult> MarkPaidFromWebhook(int id, [FromBody] MarkPaidFromWebhookDto dto)
        {
            var log = await _repository.GetByIdAsync(id);
            if (log == null) 
                return NotFound(new { message = "Usage log not found" });

            if (log.IsPaid)
                return Ok(new { message = "Already paid" });

            log.IsPaid = true;
            log.Notes = (log.Notes ?? "") + $"\nPaid via Sepay: {dto.TransactionCode} at {DateTime.Now:yyyy-MM-dd HH:mm:ss}";
            
            await _repository.UpdateAsync(log);

            return Ok(new
            {
                id = log.Id,
                isPaid = log.IsPaid,
                message = "Payment recorded successfully"
            });
        }

        private UtilityUsageLogDto MapToDto(UtilityUsageLog log)
        {
            return new UtilityUsageLogDto
            {
                Id = log.Id,
                SharedUtilityId = log.SharedUtilityId,
                UtilityName = log.SharedUtility?.Name,
                StudentId = log.StudentId,
                StudentName = log.StudentName,
                RoomNumber = log.RoomNumber,
                StartTime = log.StartTime,
                EndTime = log.EndTime,
                FeeCharged = log.FeeCharged,
                IsPaid = log.IsPaid,
                InvoiceId = log.InvoiceId,
                Notes = log.Notes,
                CreatedAt = log.CreatedAt
            };
        }
    }
}
