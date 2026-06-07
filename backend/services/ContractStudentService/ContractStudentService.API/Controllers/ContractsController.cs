using Microsoft.AspNetCore.Mvc;
using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IRoomApplicationRepository _applicationRepository;

        public ContractsController(
            IContractRepository contractRepository,
            IStudentRepository studentRepository,
            IRoomApplicationRepository applicationRepository)
        {
            _contractRepository = contractRepository;
            _studentRepository = studentRepository;
            _applicationRepository = applicationRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll()
        {
            var contracts = await _contractRepository.GetAllAsync();
            var dtos = contracts.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContractDto>> GetById(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng" });

            return Ok(MapToDto(contract));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetByStudentId(int studentId)
        {
            var contracts = await _contractRepository.GetByStudentIdAsync(studentId);
            var dtos = contracts.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetByUserId(int userId)
        {
            var contracts = await _contractRepository.GetByUserIdAsync(userId);
            var dtos = contracts.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("code/{contractCode}")]
        public async Task<ActionResult<ContractDto>> GetByContractCode(string contractCode)
        {
            var contract = await _contractRepository.GetByContractCodeAsync(contractCode);
            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng" });

            return Ok(MapToDto(contract));
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetByStatus(string status)
        {
            var contracts = await _contractRepository.GetByStatusAsync(status);
            var dtos = contracts.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<ContractDto>> Create([FromBody] CreateContractDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
                return BadRequest(new { message = "Sinh viên không tồn tại" });

            var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId);
            if (application == null)
                return BadRequest(new { message = "Đơn đăng ký không tồn tại" });

            if (application.Status != "Approved")
                return BadRequest(new { message = "Đơn đăng ký chưa được duyệt" });

            var existing = await _contractRepository.GetByContractCodeAsync(dto.ContractCode);
            if (existing != null)
                return BadRequest(new { message = "Mã hợp đồng đã tồn tại" });

            var contract = new Contract
            {
                StudentId = dto.StudentId,
                ApplicationId = dto.ApplicationId,
                RoomId = dto.RoomId,
                RoomNumber = dto.RoomNumber,
                BuildingId = dto.BuildingId,
                BuildingName = dto.BuildingName,
                RoomTypeId = dto.RoomTypeId,
                RoomTypeName = dto.RoomTypeName,
                ContractCode = dto.ContractCode,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                MonthlyRent = dto.MonthlyRent,
                DepositAmount = dto.DepositAmount,
                ElectricityRate = dto.ElectricityRate,
                WaterRate = dto.WaterRate,
                PaymentDueDay = dto.PaymentDueDay,
                WitnessName = dto.WitnessName,
                WitnessTitle = dto.WitnessTitle,
                CreatedByUserId = dto.CreatedByUserId,
                Notes = dto.Notes,
                Status = "Pending"
            };

            await _contractRepository.AddAsync(contract);

            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, MapToDto(contract));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateContractDto dto)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng" });

            contract.StartDate = dto.StartDate;
            contract.EndDate = dto.EndDate;
            contract.MonthlyRent = dto.MonthlyRent;
            contract.DepositAmount = dto.DepositAmount;
            contract.ElectricityRate = dto.ElectricityRate;
            contract.WaterRate = dto.WaterRate;
            contract.PaymentDueDay = dto.PaymentDueDay;
            contract.WitnessName = dto.WitnessName;
            contract.WitnessTitle = dto.WitnessTitle;
            contract.SignedAt = dto.SignedAt;
            contract.SignedImageUrl = dto.SignedImageUrl;
            contract.Notes = dto.Notes;

            await _contractRepository.UpdateAsync(contract);

            return NoContent();
        }

        [HttpPut("{id}/activate")]
        public async Task<ActionResult> Activate(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng" });

            contract.Status = "Active";
            contract.SignedAt = DateTime.UtcNow;

            await _contractRepository.UpdateAsync(contract);

            return NoContent();
        }

        /// <summary>
        /// Sinh viên chấp thuận hợp đồng (Pending → Active)
        /// </summary>
        [HttpPost("{id}/accept")]
        public async Task<ActionResult> AcceptContract(int id, [FromBody] AcceptContractRequest request)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng" });

            // Kiểm tra quyền: Sinh viên chỉ có thể accept hợp đồng của mình
            var student = contract.Student;
            if (student == null || student.UserId != request.UserId)
                return Forbid();

            // Kiểm tra trạng thái: Chỉ accept được hợp đồng Pending
            if (contract.Status != "Pending")
                return BadRequest(new { message = "Hợp đồng không ở trạng thái chờ chấp thuận" });

            // Cập nhật trạng thái sang Active
            contract.Status = "Active";
            contract.SignedAt = DateTime.UtcNow;

            await _contractRepository.UpdateAsync(contract);

            return Ok(new { 
                message = "Đã chấp thuận hợp đồng thành công",
                contract = MapToDto(contract)
            });
        }

        [HttpPut("{id}/terminate")]
        public async Task<ActionResult> Terminate(int id, [FromBody] TerminateContractRequest request)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng" });

            if (contract.Status == "Terminated")
                return BadRequest(new { message = "Hợp đồng đã kết thúc" });

            contract.Status = "Terminated";
            contract.TerminatedAt = DateTime.UtcNow;
            contract.TerminatedReason = request.TerminatedReason;
            contract.TerminatedByUserId = request.TerminatedByUserId;
            contract.DepositReturnedAmount = request.DepositReturnedAmount;
            contract.DepositReturnedAt = request.DepositReturnedAmount > 0 ? DateTime.UtcNow : null;
            contract.DepositDeductionReason = request.DepositDeductionReason;

            await _contractRepository.UpdateAsync(contract);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng" });

            if (contract.Status == "Active")
                return Conflict(new { message = "Không thể xóa hợp đồng đang hoạt động" });

            await _contractRepository.DeleteAsync(contract);

            return NoContent();
        }

        private static ContractDto MapToDto(Contract contract)
        {
            return new ContractDto
            {
                Id = contract.Id,
                StudentId = contract.StudentId,
                StudentName = contract.Student?.FullName,
                StudentCode = contract.Student?.StudentCode,
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
                DepositPaidAt = contract.DepositPaidAt,
                ElectricityRate = contract.ElectricityRate,
                WaterRate = contract.WaterRate,
                PaymentDueDay = contract.PaymentDueDay,
                WitnessName = contract.WitnessName,
                WitnessTitle = contract.WitnessTitle,
                SignedAt = contract.SignedAt,
                SignedImageUrl = contract.SignedImageUrl,
                Status = contract.Status,
                TerminatedAt = contract.TerminatedAt,
                TerminatedReason = contract.TerminatedReason,
                TerminatedByUserId = contract.TerminatedByUserId,
                DepositReturnedAmount = contract.DepositReturnedAmount,
                DepositReturnedAt = contract.DepositReturnedAt,
                DepositDeductionReason = contract.DepositDeductionReason,
                CreatedByUserId = contract.CreatedByUserId,
                Notes = contract.Notes,
                CreatedAt = contract.CreatedAt
            };
        }
    }

    public class UpdateContractDto
    {
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public decimal MonthlyRent { get; set; }
        public decimal DepositAmount { get; set; }
        public decimal ElectricityRate { get; set; }
        public decimal WaterRate { get; set; }
        public int PaymentDueDay { get; set; }
        public string? WitnessName { get; set; }
        public string? WitnessTitle { get; set; }
        public DateTime? SignedAt { get; set; }
        public string? SignedImageUrl { get; set; }
        public string? Notes { get; set; }
    }

    public class TerminateContractRequest
    {
        public string TerminatedReason { get; set; } = string.Empty;
        public int TerminatedByUserId { get; set; }
        public decimal DepositReturnedAmount { get; set; }
        public string? DepositDeductionReason { get; set; }
    }

    public class AcceptContractRequest
    {
        public int UserId { get; set; }
    }
}
