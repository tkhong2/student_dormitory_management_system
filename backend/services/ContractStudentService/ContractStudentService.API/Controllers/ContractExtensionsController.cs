using Microsoft.AspNetCore.Mvc;
using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractExtensionsController : ControllerBase
    {
        private readonly IContractExtensionRepository _extensionRepository;
        private readonly IContractRepository _contractRepository;

        public ContractExtensionsController(
            IContractExtensionRepository extensionRepository,
            IContractRepository contractRepository)
        {
            _extensionRepository = extensionRepository;
            _contractRepository = contractRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractExtensionDto>>> GetAll()
        {
            var extensions = await _extensionRepository.GetAllAsync();
            var dtos = extensions.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContractExtensionDto>> GetById(int id)
        {
            var extension = await _extensionRepository.GetByIdAsync(id);
            if (extension == null)
                return NotFound(new { message = "Không tìm thấy gia hạn" });

            return Ok(MapToDto(extension));
        }

        [HttpGet("contract/{contractId}")]
        public async Task<ActionResult<IEnumerable<ContractExtensionDto>>> GetByContractId(int contractId)
        {
            var extensions = await _extensionRepository.GetByContractIdAsync(contractId);
            var dtos = extensions.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<ContractExtensionDto>> Create([FromBody] CreateContractExtensionDto dto)
        {
            var contract = await _contractRepository.GetByIdAsync(dto.ContractId);
            if (contract == null)
                return BadRequest(new { message = "Hợp đồng không tồn tại" });

            if (contract.Status != "Active")
                return BadRequest(new { message = "Chỉ gia hạn được hợp đồng đang hoạt động" });

            if (dto.NewEndDate <= contract.EndDate)
                return BadRequest(new { message = "Ngày kết thúc mới phải sau ngày kết thúc hiện tại" });

            var extension = new ContractExtension
            {
                ContractId = dto.ContractId,
                OldEndDate = contract.EndDate,
                NewEndDate = dto.NewEndDate,
                Reason = dto.Reason,
                ApprovedByUserId = dto.ApprovedByUserId,
                ApprovedByName = dto.ApprovedByName,
                ApprovedAt = DateTime.UtcNow
            };

            await _extensionRepository.AddAsync(extension);

            // Update contract EndDate
            contract.EndDate = dto.NewEndDate;
            await _contractRepository.UpdateAsync(contract);

            return Ok(MapToDto(extension));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var extension = await _extensionRepository.GetByIdAsync(id);
            if (extension == null)
                return NotFound(new { message = "Không tìm thấy gia hạn" });

            await _extensionRepository.DeleteAsync(extension);

            return NoContent();
        }

        private static ContractExtensionDto MapToDto(ContractExtension ext)
        {
            return new ContractExtensionDto
            {
                Id = ext.Id,
                ContractId = ext.ContractId,
                ContractCode = ext.Contract?.ContractCode,
                StudentName = ext.Contract?.Student?.FullName,
                StudentCode = ext.Contract?.Student?.StudentCode,
                OldEndDate = ext.OldEndDate,
                NewEndDate = ext.NewEndDate,
                Reason = ext.Reason,
                ApprovedByUserId = ext.ApprovedByUserId,
                ApprovedByName = ext.ApprovedByName,
                ApprovedAt = ext.ApprovedAt
            };
        }
    }
}
