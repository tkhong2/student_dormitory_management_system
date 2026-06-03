using Microsoft.AspNetCore.Mvc;
using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomApplicationsController : ControllerBase
    {
        private readonly IRoomApplicationRepository _applicationRepository;
        private readonly IStudentRepository _studentRepository;

        public RoomApplicationsController(
            IRoomApplicationRepository applicationRepository,
            IStudentRepository studentRepository)
        {
            _applicationRepository = applicationRepository;
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomApplicationDto>>> GetAll()
        {
            var applications = await _applicationRepository.GetAllAsync();
            var dtos = applications.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomApplicationDto>> GetById(int id)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

            return Ok(MapToDto(application));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<RoomApplicationDto>>> GetByStudentId(int studentId)
        {
            var applications = await _applicationRepository.GetByStudentIdAsync(studentId);
            var dtos = applications.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<RoomApplicationDto>>> GetByStatus(string status)
        {
            var applications = await _applicationRepository.GetByStatusAsync(status);
            var dtos = applications.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<RoomApplicationDto>> Create([FromBody] CreateRoomApplicationDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
                return BadRequest(new { message = "Sinh viên không tồn tại" });

            var application = new RoomApplication
            {
                StudentId = dto.StudentId,
                PreferredBuildingId = dto.PreferredBuildingId,
                PreferredBuildingName = dto.PreferredBuildingName,
                PreferredRoomTypeId = dto.PreferredRoomTypeId,
                PreferredRoomTypeName = dto.PreferredRoomTypeName,
                PreferredRoomPrice = dto.PreferredRoomPrice,
                RequestedStartDate = dto.RequestedStartDate,
                RequestedEndDate = dto.RequestedEndDate,
                SpecialRequirements = dto.SpecialRequirements,
                Note = dto.Note,
                IsLocalStudent = dto.IsLocalStudent,
                Priority = dto.Priority,
                AttachedDocumentUrls = dto.AttachedDocumentUrls,
                Status = "Pending"
            };

            await _applicationRepository.AddAsync(application);

            return CreatedAtAction(nameof(GetById), new { id = application.Id }, MapToDto(application));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateRoomApplicationDto dto)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

            application.PreferredBuildingId = dto.PreferredBuildingId;
            application.PreferredBuildingName = dto.PreferredBuildingName;
            application.PreferredRoomTypeId = dto.PreferredRoomTypeId;
            application.PreferredRoomTypeName = dto.PreferredRoomTypeName;
            application.PreferredRoomPrice = dto.PreferredRoomPrice;
            application.RequestedStartDate = dto.RequestedStartDate;
            application.RequestedEndDate = dto.RequestedEndDate;
            application.SpecialRequirements = dto.SpecialRequirements;
            application.Note = dto.Note;
            application.IsLocalStudent = dto.IsLocalStudent;
            application.Priority = dto.Priority;
            application.AttachedDocumentUrls = dto.AttachedDocumentUrls;

            await _applicationRepository.UpdateAsync(application);

            return NoContent();
        }

        [HttpPut("{id}/approve")]
        public async Task<ActionResult> Approve(int id, [FromBody] ApproveApplicationRequest request)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

            if (application.Status != "Pending" && application.Status != "UnderReview")
                return BadRequest(new { message = "Đơn không ở trạng thái chờ duyệt" });

            application.Status = "Approved";
            application.ReviewedByUserId = request.ReviewedByUserId;
            application.ReviewedByName = request.ReviewedByName;
            application.ReviewedAt = DateTime.UtcNow;
            application.AssignedRoomId = request.AssignedRoomId;
            application.AssignedRoomNumber = request.AssignedRoomNumber;
            application.AssignedBuildingName = request.AssignedBuildingName;

            await _applicationRepository.UpdateAsync(application);

            return NoContent();
        }

        [HttpPut("{id}/reject")]
        public async Task<ActionResult> Reject(int id, [FromBody] RejectApplicationRequest request)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

            if (application.Status != "Pending" && application.Status != "UnderReview")
                return BadRequest(new { message = "Đơn không ở trạng thái chờ duyệt" });

            application.Status = "Rejected";
            application.ReviewedByUserId = request.ReviewedByUserId;
            application.ReviewedByName = request.ReviewedByName;
            application.ReviewedAt = DateTime.UtcNow;
            application.RejectReason = request.RejectReason;

            await _applicationRepository.UpdateAsync(application);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

            if (application.Contract != null)
                return Conflict(new { message = "Không thể xóa đơn đã có hợp đồng" });

            await _applicationRepository.DeleteAsync(application);

            return NoContent();
        }

        private static RoomApplicationDto MapToDto(RoomApplication app)
        {
            return new RoomApplicationDto
            {
                Id = app.Id,
                StudentId = app.StudentId,
                StudentName = app.Student?.FullName,
                StudentCode = app.Student?.StudentCode,
                PreferredBuildingId = app.PreferredBuildingId,
                PreferredBuildingName = app.PreferredBuildingName,
                PreferredRoomTypeId = app.PreferredRoomTypeId,
                PreferredRoomTypeName = app.PreferredRoomTypeName,
                PreferredRoomPrice = app.PreferredRoomPrice,
                RequestedStartDate = app.RequestedStartDate,
                RequestedEndDate = app.RequestedEndDate,
                SpecialRequirements = app.SpecialRequirements,
                Note = app.Note,
                IsLocalStudent = app.IsLocalStudent,
                Priority = app.Priority,
                AttachedDocumentUrls = app.AttachedDocumentUrls,
                Status = app.Status,
                ReviewedByUserId = app.ReviewedByUserId,
                ReviewedByName = app.ReviewedByName,
                ReviewedAt = app.ReviewedAt,
                RejectReason = app.RejectReason,
                AssignedRoomId = app.AssignedRoomId,
                AssignedRoomNumber = app.AssignedRoomNumber,
                AssignedBuildingName = app.AssignedBuildingName,
                CancelledAt = app.CancelledAt,
                CancelledReason = app.CancelledReason,
                CancelledBySelf = app.CancelledBySelf,
                CreatedAt = app.CreatedAt
            };
        }
    }

    public class ApproveApplicationRequest
    {
        public int ReviewedByUserId { get; set; }
        public string ReviewedByName { get; set; } = string.Empty;
        public int AssignedRoomId { get; set; }
        public string AssignedRoomNumber { get; set; } = string.Empty;
        public string AssignedBuildingName { get; set; } = string.Empty;
    }

    public class RejectApplicationRequest
    {
        public int ReviewedByUserId { get; set; }
        public string ReviewedByName { get; set; } = string.Empty;
        public string RejectReason { get; set; } = string.Empty;
    }
}
