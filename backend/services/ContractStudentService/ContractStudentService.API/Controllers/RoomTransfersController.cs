using Microsoft.AspNetCore.Mvc;
using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomTransfersController : ControllerBase
    {
        private readonly IRoomTransferRepository _transferRepository;
        private readonly IContractRepository _contractRepository;

        public RoomTransfersController(
            IRoomTransferRepository transferRepository,
            IContractRepository contractRepository)
        {
            _transferRepository = transferRepository;
            _contractRepository = contractRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomTransferDto>>> GetAll()
        {
            var transfers = await _transferRepository.GetAllAsync();
            var dtos = transfers.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomTransferDto>> GetById(int id)
        {
            var transfer = await _transferRepository.GetByIdAsync(id);
            if (transfer == null)
                return NotFound(new { message = "Không tìm thấy đơn chuyển phòng" });

            return Ok(MapToDto(transfer));
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<RoomTransferDto>>> GetByStudentId(int studentId)
        {
            var transfers = await _transferRepository.GetByStudentIdAsync(studentId);
            var dtos = transfers.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<RoomTransferDto>>> GetByStatus(string status)
        {
            var transfers = await _transferRepository.GetByStatusAsync(status);
            var dtos = transfers.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<RoomTransferDto>> Create([FromBody] CreateRoomTransferDto dto)
        {
            var contract = await _contractRepository.GetByIdAsync(dto.ContractId);
            if (contract == null)
                return BadRequest(new { message = "Hợp đồng không tồn tại" });

            if (contract.Status != "Active")
                return BadRequest(new { message = "Chỉ chuyển phòng cho hợp đồng đang hoạt động" });

            var transfer = new RoomTransfer
            {
                ContractId = dto.ContractId,
                StudentId = dto.StudentId,
                CurrentRoomId = dto.CurrentRoomId,
                CurrentRoomNumber = dto.CurrentRoomNumber,
                RequestedRoomTypeId = dto.RequestedRoomTypeId,
                RequestedRoomTypeName = dto.RequestedRoomTypeName,
                RequestedBuildingId = dto.RequestedBuildingId,
                RequestedBuildingName = dto.RequestedBuildingName,
                Reason = dto.Reason,
                Status = "Pending"
            };

            await _transferRepository.AddAsync(transfer);

            return CreatedAtAction(nameof(GetById), new { id = transfer.Id }, MapToDto(transfer));
        }

        [HttpPut("{id}/approve")]
        public async Task<ActionResult> Approve(int id, [FromBody] ApproveTransferRequest request)
        {
            var transfer = await _transferRepository.GetByIdAsync(id);
            if (transfer == null)
                return NotFound(new { message = "Không tìm thấy đơn chuyển phòng" });

            if (transfer.Status != "Pending")
                return BadRequest(new { message = "Đơn không ở trạng thái chờ duyệt" });

            transfer.Status = "Approved";
            transfer.NewRoomId = request.NewRoomId;
            transfer.NewRoomNumber = request.NewRoomNumber;
            transfer.TransferDate = request.TransferDate;
            transfer.ReviewedByUserId = request.ReviewedByUserId;
            transfer.ReviewedByName = request.ReviewedByName;
            transfer.ReviewedAt = DateTime.UtcNow;

            await _transferRepository.UpdateAsync(transfer);

            // Update contract room info
            var contract = transfer.Contract;
            contract.RoomId = request.NewRoomId;
            contract.RoomNumber = request.NewRoomNumber;
            await _contractRepository.UpdateAsync(contract);

            return NoContent();
        }

        [HttpPut("{id}/reject")]
        public async Task<ActionResult> Reject(int id, [FromBody] RejectTransferRequest request)
        {
            var transfer = await _transferRepository.GetByIdAsync(id);
            if (transfer == null)
                return NotFound(new { message = "Không tìm thấy đơn chuyển phòng" });

            if (transfer.Status != "Pending")
                return BadRequest(new { message = "Đơn không ở trạng thái chờ duyệt" });

            transfer.Status = "Rejected";
            transfer.ReviewedByUserId = request.ReviewedByUserId;
            transfer.ReviewedByName = request.ReviewedByName;
            transfer.ReviewedAt = DateTime.UtcNow;
            transfer.RejectReason = request.RejectReason;

            await _transferRepository.UpdateAsync(transfer);

            return NoContent();
        }

        private static RoomTransferDto MapToDto(RoomTransfer transfer)
        {
            return new RoomTransferDto
            {
                Id = transfer.Id,
                ContractId = transfer.ContractId,
                StudentId = transfer.StudentId,
                StudentName = transfer.Contract?.Student?.FullName,
                StudentCode = transfer.Contract?.Student?.StudentCode,
                CurrentRoomId = transfer.CurrentRoomId,
                CurrentRoomNumber = transfer.CurrentRoomNumber,
                RequestedRoomTypeId = transfer.RequestedRoomTypeId,
                RequestedRoomTypeName = transfer.RequestedRoomTypeName,
                RequestedBuildingId = transfer.RequestedBuildingId,
                RequestedBuildingName = transfer.RequestedBuildingName,
                Reason = transfer.Reason,
                Status = transfer.Status,
                NewRoomId = transfer.NewRoomId,
                NewRoomNumber = transfer.NewRoomNumber,
                TransferDate = transfer.TransferDate,
                ReviewedByUserId = transfer.ReviewedByUserId,
                ReviewedByName = transfer.ReviewedByName,
                ReviewedAt = transfer.ReviewedAt,
                RejectReason = transfer.RejectReason,
                CreatedAt = transfer.CreatedAt
            };
        }
    }

    public class ApproveTransferRequest
    {
        public int NewRoomId { get; set; }
        public string NewRoomNumber { get; set; } = string.Empty;
        public DateOnly TransferDate { get; set; }
        public int ReviewedByUserId { get; set; }
        public string ReviewedByName { get; set; } = string.Empty;
    }

    public class RejectTransferRequest
    {
        public int ReviewedByUserId { get; set; }
        public string ReviewedByName { get; set; } = string.Empty;
        public string RejectReason { get; set; } = string.Empty;
    }
}
