using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;
using ContractStudentService.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractsController : ControllerBase
    {
        private readonly IContractRepository _contractRepository;

        public ContractsController(IContractRepository contractRepository)
        {
            _contractRepository = contractRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractDto>>> GetAll()
        {
            var contracts = await _contractRepository.GetAllAsync();
            return Ok(contracts.Select(ToDto));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ContractDto>> GetById(Guid id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Contract not found." });

            return Ok(ToDto(contract));
        }

        [HttpPost]
        public async Task<ActionResult<ContractDto>> Create([FromBody] CreateContractDto dto)
        {
            var validationResult = Validate(dto, out var status);
            if (validationResult != null)
                return validationResult;

            var contract = ToEntity(dto, status);
            await _contractRepository.AddAsync(contract);

            return CreatedAtAction(nameof(GetById), new { id = contract.Id }, ToDto(contract));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateContractDto dto)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Contract not found." });

            var validationResult = Validate(dto, out var status);
            if (validationResult != null)
                return validationResult;

            var updated = ToEntity(dto, status);
            updated.Id = id;
            updated.CreatedAt = contract.CreatedAt;

            await _contractRepository.UpdateAsync(updated);
            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var contract = await _contractRepository.GetByIdAsync(id);
            if (contract == null)
                return NotFound(new { message = "Contract not found." });

            await _contractRepository.DeleteAsync(contract);
            return NoContent();
        }

        private BadRequestObjectResult? Validate(CreateContractDto dto, out ContractStatus status)
        {
            status = default;
            if (string.IsNullOrWhiteSpace(dto.Code) ||
                string.IsNullOrWhiteSpace(dto.StudentCode) ||
                string.IsNullOrWhiteSpace(dto.StudentName) ||
                string.IsNullOrWhiteSpace(dto.RoomNumber))
            {
                return BadRequest(new { message = "Code, student and room information are required." });
            }

            if (dto.StudentId == Guid.Empty || dto.RoomId == Guid.Empty)
                return BadRequest(new { message = "StudentId and roomId are required." });

            if (dto.EndDate.Date < dto.StartDate.Date)
                return BadRequest(new { message = "End date must not be earlier than start date." });

            if (dto.Price < 0)
                return BadRequest(new { message = "Price must not be negative." });

            if (!Enum.TryParse(dto.Status, true, out status))
                return BadRequest(new { message = "Contract status is invalid." });

            return null;
        }

        private static Contract ToEntity(CreateContractDto dto, ContractStatus status)
        {
            return new Contract
            {
                Code = dto.Code.Trim(),
                StudentId = dto.StudentId,
                Student = new Student
                {
                    Id = dto.StudentId,
                    StudentCode = dto.StudentCode.Trim(),
                    FullName = dto.StudentName.Trim()
                },
                RoomId = dto.RoomId,
                RoomNumber = dto.RoomNumber.Trim(),
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                Price = dto.Price,
                Status = status
            };
        }

        private static ContractDto ToDto(Contract contract)
        {
            return new ContractDto
            {
                Id = contract.Id,
                Code = contract.Code,
                StudentId = contract.StudentId,
                StudentCode = contract.Student?.StudentCode ?? string.Empty,
                StudentName = contract.Student?.FullName ?? string.Empty,
                RoomId = contract.RoomId,
                RoomNumber = contract.RoomNumber,
                StartDate = contract.StartDate,
                EndDate = contract.EndDate,
                Price = contract.Price,
                Status = contract.Status.ToString(),
                CreatedAt = contract.CreatedAt
            };
        }
    }
}
