using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private static readonly string[] AllowedStatuses = ["Active", "Expiring", "Departed"];
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        {
            var students = await _studentRepository.GetAllAsync();
            return Ok(students.Select(ToDto));
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<StudentDto>> GetById(Guid id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            return Ok(ToDto(student));
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto dto)
        {
            var validationResult = Validate(dto);
            if (validationResult != null)
                return validationResult;

            var student = ToEntity(dto);
            await _studentRepository.AddAsync(student);

            return CreatedAtAction(nameof(GetById), new { id = student.Id }, ToDto(student));
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CreateStudentDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            var validationResult = Validate(dto);
            if (validationResult != null)
                return validationResult;

            var updated = ToEntity(dto);
            updated.Id = id;
            await _studentRepository.UpdateAsync(updated);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Student not found." });

            await _studentRepository.DeleteAsync(student);
            return NoContent();
        }

        private BadRequestObjectResult? Validate(CreateStudentDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.StudentCode) ||
                string.IsNullOrWhiteSpace(dto.FullName) ||
                string.IsNullOrWhiteSpace(dto.RoomNumber) ||
                string.IsNullOrWhiteSpace(dto.BuildingName) ||
                string.IsNullOrWhiteSpace(dto.ClassName))
            {
                return BadRequest(new { message = "Student code, name and residence information are required." });
            }

            if (dto.JoinDate == default)
                return BadRequest(new { message = "Join date is required." });

            if (!AllowedStatuses.Contains(dto.Status, StringComparer.OrdinalIgnoreCase))
                return BadRequest(new { message = "Student status is invalid." });

            return null;
        }

        private static Student ToEntity(CreateStudentDto dto)
        {
            return new Student
            {
                StudentCode = dto.StudentCode.Trim(),
                FullName = dto.FullName.Trim(),
                Email = dto.Email.Trim(),
                PhoneNumber = dto.PhoneNumber.Trim(),
                Address = dto.Address.Trim(),
                DateOfBirth = dto.DateOfBirth ?? default,
                Gender = dto.Gender.Trim(),
                RoomNumber = dto.RoomNumber.Trim(),
                BuildingName = dto.BuildingName.Trim(),
                ClassName = dto.ClassName.Trim(),
                JoinDate = dto.JoinDate,
                Status = dto.Status.Trim()
            };
        }

        private static StudentDto ToDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber,
                Address = student.Address,
                DateOfBirth = student.DateOfBirth,
                Gender = student.Gender,
                RoomNumber = student.RoomNumber,
                BuildingName = student.BuildingName,
                ClassName = student.ClassName,
                JoinDate = student.JoinDate,
                Status = student.Status
            };
        }
    }
}
