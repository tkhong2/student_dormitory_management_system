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
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        private readonly ContractStudentService.Application.Services.IRoomServiceClient _roomServiceClient;

        public ContractsController(
            IContractRepository contractRepository,
            IStudentRepository studentRepository,
            IRoomApplicationRepository applicationRepository,
            IHttpClientFactory httpClientFactory,
            IConfiguration configuration,
            ContractStudentService.Application.Services.IRoomServiceClient roomServiceClient)
        {
            _contractRepository = contractRepository;
            _studentRepository = studentRepository;
            _applicationRepository = applicationRepository;
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
            _roomServiceClient = roomServiceClient;
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

            // ApplicationId là optional - chỉ validate nếu được cung cấp
            if (dto.ApplicationId.HasValue && dto.ApplicationId.Value > 0)
            {
                var application = await _applicationRepository.GetByIdAsync(dto.ApplicationId.Value);
                if (application == null)
                    return BadRequest(new { message = "Đơn đăng ký không tồn tại" });

                if (application.Status != "Approved")
                    return BadRequest(new { message = "Đơn đăng ký chưa được duyệt" });
            }

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
                Status = "PendingDeposit",  // Chờ đóng cọc (thay vì Pending)
                IsDepositPaid = false
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
            if (!string.IsNullOrEmpty(dto.Status))
            {
                contract.Status = dto.Status;
            }

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
        /// Staff xác nhận sinh viên đã đóng tiền cọc (PendingDeposit → Active)
        /// </summary>
        [HttpPost("{id}/confirm-deposit")]
        public async Task<ActionResult> ConfirmDeposit(int id, [FromBody] ConfirmDepositRequest request)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Không tìm thấy hợp đồng" });

            // Kiểm tra trạng thái: Chỉ confirm được hợp đồng PendingDeposit
            if (contract.Status != "PendingDeposit")
                return BadRequest(new { message = "Hợp đồng không ở trạng thái chờ đóng cọc" });

            // Cập nhật trạng thái
            contract.Status = "Active";
            contract.IsDepositPaid = true;
            contract.DepositPaidAt = DateTime.UtcNow;

            await _contractRepository.UpdateAsync(contract);

            // Tự động sinh công nợ tháng đầu tiên khi hợp đồng Active
            await GenerateMonthlyInvoice(contract);

            return Ok(new { 
                message = "Đã xác nhận đóng cọc thành công. Hợp đồng đã được kích hoạt và sinh công nợ tháng đầu tiên.",
                contract = MapToDto(contract)
            });
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

            // Giảm số người trong phòng (-1 sinh viên) và tự động cập nhật trạng thái
            var roomUpdateSuccess = await _roomServiceClient.UpdateRoomOccupancyAsync(
                contract.RoomId, 
                increment: -1 // Giảm 1 người
            );

            if (!roomUpdateSuccess)
            {
                Console.WriteLine($"[WARNING] Không thể cập nhật occupancy cho phòng {contract.RoomNumber}");
            }

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

        /// <summary>
        /// Tự động sinh công nợ hàng tháng cho hợp đồng Active
        /// </summary>
        private async Task GenerateMonthlyInvoice(Contract contract)
        {
            try
            {
                var student = contract.Student;
                if (student == null) return;

                var billingServiceUrl = _configuration["Services:BillingService"] ?? "http://localhost:5002";
                var httpClient = _httpClientFactory.CreateClient();

                // Tính toán kỳ thanh toán (tháng hiện tại)
                var now = DateTime.Now;
                var billingMonth = now.Month;
                var billingYear = now.Year;

                // Tạo mã phiếu thu: PTT<YYYYMM><ContractId>
                var invoiceCode = $"PTT{billingYear:0000}{billingMonth:00}{contract.Id:000}";

                // Tính hạn thanh toán (ngày PaymentDueDay của tháng hiện tại)
                var dueDate = new DateOnly(billingYear, billingMonth, contract.PaymentDueDay);

                // Prepare invoice data
                var invoiceData = new
                {
                    invoiceCode = invoiceCode,
                    invoiceType = "Monthly",
                    contractId = contract.Id,
                    studentId = contract.StudentId,
                    studentName = student.FullName,
                    studentCode = student.StudentCode,
                    roomId = contract.RoomId,
                    roomNumber = contract.RoomNumber,
                    buildingName = contract.BuildingName,
                    billingMonth = billingMonth,
                    billingYear = billingYear,
                    rentAmount = contract.MonthlyRent,
                    electricityAmount = 0m, // Sẽ cập nhật sau khi có chỉ số điện
                    waterAmount = 0m,       // Sẽ cập nhật sau khi có chỉ số nước
                    serviceAmount = 0m,     // Phí dịch vụ khác (nếu có)
                    previousDebt = 0m,      // Nợ kỳ trước
                    discount = 0m,
                    dueDate = dueDate,
                    createdByUserId = 1,    // System auto-generate
                    notes = "Công nợ tháng được sinh tự động từ hợp đồng",
                    items = new[]
                    {
                        new
                        {
                            itemName = "Tiền phòng",
                            itemDescription = $"Tháng {billingMonth}/{billingYear}",
                            quantity = 1m,
                            unit = "tháng",
                            unitPrice = contract.MonthlyRent,
                            sortOrder = 1
                        }
                    }
                };

                var response = await httpClient.PostAsJsonAsync($"{billingServiceUrl}/api/invoices", invoiceData);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Log error but don't throw - sinh công nợ thất bại không nên block xác nhận hợp đồng
                Console.WriteLine($"Error generating invoice: {ex.Message}");
            }
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
                ContractTemplateId = contract.ContractTemplateId,
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
                CreatedAt = contract.CreatedAt,
                Terms = contract.ContractTemplate?.Terms?
                    .OrderBy(t => t.OrderIndex)
                    .Select(t => new ContractTermDto
                    {
                        Id = t.Id,
                        ContractTemplateId = t.ContractTemplateId,
                        Title = t.Title,
                        Content = t.Content,
                        OrderIndex = t.OrderIndex,
                        IsRequired = t.IsRequired,
                        IsHighlighted = t.IsHighlighted,
                        Icon = t.Icon
                    }).ToList() ?? new List<ContractTermDto>()
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
        public string? Status { get; set; }
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

    public class ConfirmDepositRequest
    {
        public int ConfirmedByUserId { get; set; }
        public string? ConfirmedByName { get; set; }
    }
}
