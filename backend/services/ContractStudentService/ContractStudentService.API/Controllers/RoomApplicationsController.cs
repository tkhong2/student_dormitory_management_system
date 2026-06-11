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
        private readonly IContractRepository _contractRepository;
        private readonly IContractTemplateRepository _templateRepository;

        public RoomApplicationsController(
            IRoomApplicationRepository applicationRepository,
            IStudentRepository studentRepository,
            IContractRepository contractRepository,
            IContractTemplateRepository templateRepository)
        {
            _applicationRepository = applicationRepository;
            _studentRepository = studentRepository;
            _contractRepository = contractRepository;
            _templateRepository = templateRepository;
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

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<RoomApplicationDto>>> GetByUserId(int userId)
        {
            var applications = await _applicationRepository.GetByUserIdAsync(userId);
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
            // Validate required fields
            if (dto.StudentId <= 0)
                return BadRequest(new { message = "StudentId không hợp lệ" });

            if (dto.AssignedRoomId <= 0 || string.IsNullOrEmpty(dto.AssignedRoomNumber))
                return BadRequest(new { message = "Phải chọn phòng cụ thể" });

            // Check if Student exists by UserId (not by StudentId = Id)
            var students = await _studentRepository.GetAllAsync();
            var student = students.FirstOrDefault(s => s.UserId == dto.StudentId);
            
            if (student == null)
            {
                // Auto-create Student record if not exists - use user info from frontend
                student = new Student
                {
                    UserId = dto.StudentId,
                    StudentCode = $"SV{dto.StudentId:D6}",
                    FullName = !string.IsNullOrEmpty(dto.UserFullName) ? dto.UserFullName : "Chưa cập nhật",
                    Gender = "Unknown",
                    DateOfBirth = new DateOnly(2000, 1, 1),
                    Phone = !string.IsNullOrEmpty(dto.UserPhone) ? dto.UserPhone : "",
                    Email = !string.IsNullOrEmpty(dto.UserEmail) ? dto.UserEmail : "",
                    IdentityCard = "",
                    IdentityCardIssuedDate = DateOnly.FromDateTime(DateTime.Now),
                    IdentityCardIssuedPlace = "",
                    PermanentAddress = "",
                    EmergencyContactName = "",
                    EmergencyContactPhone = "",
                    EmergencyContactRelation = "",
                    Faculty = "Chưa cập nhật",
                    Major = "Chưa cập nhật",
                    AcademicYear = 1,
                    IsActive = true,
                    ProfileCompletionPct = 10
                };
                
                await _studentRepository.AddAsync(student);
            }
            
            // Use the actual Student.Id (auto-generated) for the application
            var actualStudentId = student.Id;

            // Check for existing active applications
            var existingApplications = await _applicationRepository.GetActiveApplicationsByStudentAsync(actualStudentId);
            if (existingApplications.Any())
                return Conflict(new { message = "Sinh viên đã có đơn đang chờ duyệt hoặc đã được duyệt" });

            var application = new RoomApplication
            {
                StudentId = actualStudentId,
                PreferredBuildingId = dto.PreferredBuildingId,
                PreferredBuildingName = dto.PreferredBuildingName,
                PreferredRoomTypeId = dto.PreferredRoomTypeId,
                PreferredRoomTypeName = dto.PreferredRoomTypeName,
                PreferredRoomPrice = dto.PreferredRoomPrice,
                RequestedStartDate = dto.RequestedStartDate,
                RequestedEndDate = dto.RequestedEndDate,
                DurationMonths = dto.DurationMonths,
                Preferences = dto.Preferences,
                Note = dto.Note,
                IsLocalStudent = dto.IsLocalStudent,
                Priority = dto.IsLocalStudent ? 0 : 1, // 0 = local, 1 = non-local
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyContactPhone = dto.EmergencyContactPhone,
                EmergencyContactRelationship = dto.EmergencyContactRelationship,
                AgreedToRegulations = dto.AgreedToRegulations,
                ConfirmedInformationAccuracy = dto.ConfirmedInformationAccuracy,
                AttachedDocumentUrls = dto.AttachedDocumentUrls,
                Status = "Pending",
                // Lưu luôn thông tin phòng sinh viên đã chọn
                AssignedRoomId = dto.AssignedRoomId,
                AssignedRoomNumber = dto.AssignedRoomNumber,
                AssignedBuildingName = dto.AssignedBuildingName ?? dto.PreferredBuildingName
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
            application.DurationMonths = dto.DurationMonths;
            application.Preferences = dto.Preferences;
            application.Note = dto.Note;
            application.IsLocalStudent = dto.IsLocalStudent;
            application.Priority = dto.IsLocalStudent ? 0 : 1; // Auto-set based on IsLocalStudent
            application.EmergencyContactName = dto.EmergencyContactName;
            application.EmergencyContactPhone = dto.EmergencyContactPhone;
            application.EmergencyContactRelationship = dto.EmergencyContactRelationship;
            application.AgreedToRegulations = dto.AgreedToRegulations;
            application.ConfirmedInformationAccuracy = dto.ConfirmedInformationAccuracy;
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

            // Kiểm tra đơn đã có thông tin phòng chưa
            if (!application.AssignedRoomId.HasValue || string.IsNullOrEmpty(application.AssignedRoomNumber))
                return BadRequest(new { message = "Đơn chưa có thông tin phòng được chọn" });

            // Cập nhật trạng thái đơn sang Approved
            application.Status = "Approved";
            application.ReviewedByUserId = request.ReviewedByUserId;
            application.ReviewedByName = request.ReviewedByName;
            application.ReviewedAt = DateTime.UtcNow;

            await _applicationRepository.UpdateAsync(application);

            // Lấy default contract template
            var defaultTemplate = await _templateRepository.GetDefaultAsync();
            if (defaultTemplate == null)
            {
                return StatusCode(500, new { message = "Không tìm thấy mẫu hợp đồng mặc định trong hệ thống" });
            }

            // Tự động tạo hợp đồng nháp (Pending) để sinh viên xem và chấp thuận
            var year = DateTime.UtcNow.Year;
            var sequence = await _contractRepository.GetNextSequenceForYearAsync(year);
            var contractCode = $"HD{year}{sequence:D4}";

            var contract = new Contract
            {
                StudentId = application.StudentId,
                ApplicationId = application.Id,
                ContractTemplateId = defaultTemplate.Id,
                RoomId = application.AssignedRoomId ?? 0,
                RoomNumber = application.AssignedRoomNumber ?? "",
                BuildingId = application.PreferredBuildingId,
                BuildingName = application.AssignedBuildingName ?? application.PreferredBuildingName,
                RoomTypeId = application.PreferredRoomTypeId,
                RoomTypeName = application.PreferredRoomTypeName,
                ContractCode = contractCode,
                StartDate = application.RequestedStartDate,
                EndDate = application.RequestedEndDate,
                MonthlyRent = application.PreferredRoomPrice,
                DepositAmount = application.PreferredRoomPrice, // Default: 1 tháng tiền phòng
                ElectricityRate = 3500, // Default rate
                WaterRate = 15000, // Default rate
                PaymentDueDay = 5, // Ngày 5 hàng tháng
                Status = "Pending", // Chờ sinh viên chấp thuận
                CreatedByUserId = request.ReviewedByUserId,
                CreatedAt = DateTime.UtcNow
            };

            await _contractRepository.AddAsync(contract);

            return Ok(new { message = "Đã duyệt đơn và tạo hợp đồng nháp thành công", application = MapToDto(application) });
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

            // Kiểm tra xem đơn đã có hợp đồng chưa
            var contracts = await _contractRepository.GetAllAsync();
            var relatedContracts = contracts.Where(c => c.ApplicationId == application.Id).ToList();
            
            if (relatedContracts.Any())
            {
                // Xóa tất cả hợp đồng liên quan trước
                foreach (var contract in relatedContracts)
                {
                    // Chỉ cho phép xóa hợp đồng Pending hoặc chưa Active
                    if (contract.Status == "Active")
                    {
                        return Conflict(new { message = "Không thể xóa đơn có hợp đồng đang hoạt động. Vui lòng chấm dứt hợp đồng trước." });
                    }
                    
                    await _contractRepository.DeleteAsync(contract);
                }
            }

            await _applicationRepository.DeleteAsync(application);

            return NoContent();
        }

        /// <summary>
        /// Sinh viên chấp thuận đơn đã được duyệt → Tự động tạo hợp đồng
        /// </summary>
        [HttpPost("{id}/accept")]
        public async Task<ActionResult<ContractDto>> AcceptApplication(int id, [FromBody] AcceptApplicationRequest request)
        {
            // 1. Lấy đơn đăng ký
            var application = await _applicationRepository.GetByIdAsync(id);
            if (application == null)
                return NotFound(new { message = "Không tìm thấy đơn đăng ký" });

            // 2. Kiểm tra quyền sở hữu
            if (application.StudentId != request.StudentId)
                return Forbid("Đơn không thuộc về sinh viên này");

            // 3. Kiểm tra trạng thái đơn phải là Approved
            if (application.Status != "Approved")
                return BadRequest(new { message = "Đơn chưa được duyệt" });

            // 4. Kiểm tra sinh viên đã có hợp đồng Active chưa
            var existingContracts = await _contractRepository.GetActiveContractsByStudentAsync(request.StudentId);
            if (existingContracts.Any())
                return Conflict(new { message = "Sinh viên đã có hợp đồng đang hoạt động" });

            // 5. Lấy default contract template
            var defaultTemplate = await _templateRepository.GetDefaultAsync();
            if (defaultTemplate == null)
            {
                return StatusCode(500, new { message = "Không tìm thấy mẫu hợp đồng mặc định trong hệ thống" });
            }

            // 6. Tạo mã hợp đồng: HD{YEAR}{SEQUENCE}
            var year = DateTime.UtcNow.Year;
            var sequence = await _contractRepository.GetNextSequenceForYearAsync(year);
            var contractCode = $"HD{year}{sequence:D4}";

            // 7. Tạo hợp đồng mới (với default values - có thể lấy từ SystemSettings sau)
            var contract = new Contract
            {
                StudentId = application.StudentId,
                ApplicationId = application.Id,
                ContractTemplateId = defaultTemplate.Id,
                RoomId = application.AssignedRoomId ?? 0,
                RoomNumber = application.AssignedRoomNumber ?? "",
                BuildingId = application.PreferredBuildingId,
                BuildingName = application.AssignedBuildingName ?? application.PreferredBuildingName,
                RoomTypeId = application.PreferredRoomTypeId,
                RoomTypeName = application.PreferredRoomTypeName,
                ContractCode = contractCode,
                StartDate = application.RequestedStartDate,
                EndDate = application.RequestedEndDate,
                MonthlyRent = application.PreferredRoomPrice,
                DepositAmount = application.PreferredRoomPrice, // Default: 1 tháng tiền phòng
                ElectricityRate = 3500, // Default rate
                WaterRate = 15000, // Default rate
                PaymentDueDay = 5, // Ngày 5 hàng tháng
                Status = "Pending", // Chờ ký
                CreatedByUserId = request.StudentId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                // 8. Lưu hợp đồng
                await _contractRepository.AddAsync(contract);

                // 9. Link hợp đồng vào đơn
                // application.Contract = contract; // EF sẽ tự động link qua ApplicationId
                await _applicationRepository.UpdateAsync(application);

                // 10. Trả về DTO
                var contractDto = new ContractDto
                {
                    Id = contract.Id,
                    StudentId = contract.StudentId,
                    ApplicationId = contract.ApplicationId,
                    RoomId = contract.RoomId,
                    RoomNumber = contract.RoomNumber,
                    BuildingId = contract.BuildingId,
                    BuildingName = contract.BuildingName,
                    RoomTypeId = contract.RoomTypeId,
                    RoomTypeName = contract.RoomTypeName,
                    ContractCode = contract.ContractCode,
                    StartDate = contract.StartDate,
                    EndDate = contract.EndDate,
                    MonthlyRent = contract.MonthlyRent,
                    DepositAmount = contract.DepositAmount,
                    IsDepositPaid = contract.IsDepositPaid,
                    ElectricityRate = contract.ElectricityRate,
                    WaterRate = contract.WaterRate,
                    PaymentDueDay = contract.PaymentDueDay,
                    Status = contract.Status,
                    CreatedAt = contract.CreatedAt
                };

                return CreatedAtAction("GetById", "Contracts", new { id = contract.Id }, contractDto);
            }
            catch (Exception ex)
            {
                // Log error và giữ đơn ở trạng thái Approved để có thể retry
                return StatusCode(500, new { message = "Lỗi tạo hợp đồng", error = ex.Message });
            }
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
                DurationMonths = app.DurationMonths,
                Preferences = app.Preferences,
                Note = app.Note,
                IsLocalStudent = app.IsLocalStudent,
                Priority = app.Priority,
                EmergencyContactName = app.EmergencyContactName,
                EmergencyContactPhone = app.EmergencyContactPhone,
                EmergencyContactRelationship = app.EmergencyContactRelationship,
                AgreedToRegulations = app.AgreedToRegulations,
                ConfirmedInformationAccuracy = app.ConfirmedInformationAccuracy,
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
    }

    public class RejectApplicationRequest
    {
        public int ReviewedByUserId { get; set; }
        public string ReviewedByName { get; set; } = string.Empty;
        public string RejectReason { get; set; } = string.Empty;
    }

    public class AcceptApplicationRequest
    {
        public int StudentId { get; set; }
    }
}
