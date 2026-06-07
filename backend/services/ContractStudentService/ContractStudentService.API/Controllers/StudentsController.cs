using Microsoft.AspNetCore.Mvc;
using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentRepository _studentRepository;

        public StudentsController(IStudentRepository studentRepository)
        {
            _studentRepository = studentRepository;
        }

        [HttpGet("by-user/{userId}")]
    public async Task<ActionResult<StudentDto>> GetByUserId(int userId)
    {
        var students = await _studentRepository.GetAllAsync();
        var student = students.FirstOrDefault(s => s.UserId == userId);
        
        if (student == null)
            return NotFound(new { message = $"Không tìm thấy sinh viên với UserId = {userId}" });

        return Ok(MapToDto(student));
    }

    [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        {
            var students = await _studentRepository.GetAllAsync();
            var dtos = students.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDto>> GetById(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Không tìm thấy sinh viên" });

            return Ok(MapToDto(student));
        }

        [HttpGet("code/{studentCode}")]
        public async Task<ActionResult<StudentDto>> GetByStudentCode(string studentCode)
        {
            var student = await _studentRepository.GetByStudentCodeAsync(studentCode);
            if (student == null)
                return NotFound(new { message = "Không tìm thấy sinh viên" });

            return Ok(MapToDto(student));
        }

        [HttpPost]
        public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto dto)
        {
            var existing = await _studentRepository.GetByStudentCodeAsync(dto.StudentCode);
            if (existing != null)
                return BadRequest(new { message = "Mã sinh viên đã tồn tại" });

            var student = new Student
            {
                StudentCode = dto.StudentCode,
                Faculty = dto.Faculty,
                Major = dto.Major,
                AcademicYear = dto.AcademicYear,
                ClassCode = dto.ClassCode,
                FullName = dto.FullName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                PlaceOfBirth = dto.PlaceOfBirth,
                Ethnicity = dto.Ethnicity,
                Religion = dto.Religion,
                Nationality = dto.Nationality,
                BloodType = dto.BloodType,
                HealthInsuranceNumber = dto.HealthInsuranceNumber,
                Phone = dto.Phone,
                Email = dto.Email,
                IdentityCard = dto.IdentityCard,
                IdentityCardIssuedDate = dto.IdentityCardIssuedDate,
                IdentityCardIssuedPlace = dto.IdentityCardIssuedPlace,
                PermanentAddress = dto.PermanentAddress,
                PermanentProvince = dto.PermanentProvince,
                TemporaryAddress = dto.TemporaryAddress,
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyContactPhone = dto.EmergencyContactPhone,
                EmergencyContactRelation = dto.EmergencyContactRelation,
                EmergencyContactAddress = dto.EmergencyContactAddress,
                AvatarUrl = dto.AvatarUrl,
                IsActive = dto.IsActive,
                Notes = dto.Notes,
                UserId = dto.UserId,
                ProfileCompletionPct = CalculateProfileCompletion(dto)
            };

            await _studentRepository.AddAsync(student);

            return CreatedAtAction(nameof(GetById), new { id = student.Id }, MapToDto(student));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateStudentDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Không tìm thấy sinh viên" });

            student.StudentCode = dto.StudentCode;
            student.Faculty = dto.Faculty;
            student.Major = dto.Major;
            student.AcademicYear = dto.AcademicYear;
            student.ClassCode = dto.ClassCode;
            student.FullName = dto.FullName;
            student.Gender = dto.Gender;
            student.DateOfBirth = dto.DateOfBirth;
            student.PlaceOfBirth = dto.PlaceOfBirth;
            student.Ethnicity = dto.Ethnicity;
            student.Religion = dto.Religion;
            student.Nationality = dto.Nationality;
            student.BloodType = dto.BloodType;
            student.HealthInsuranceNumber = dto.HealthInsuranceNumber;
            student.Phone = dto.Phone;
            student.Email = dto.Email;
            student.IdentityCard = dto.IdentityCard;
            student.IdentityCardIssuedDate = dto.IdentityCardIssuedDate;
            student.IdentityCardIssuedPlace = dto.IdentityCardIssuedPlace;
            student.PermanentAddress = dto.PermanentAddress;
            student.PermanentProvince = dto.PermanentProvince;
            student.TemporaryAddress = dto.TemporaryAddress;
            student.EmergencyContactName = dto.EmergencyContactName;
            student.EmergencyContactPhone = dto.EmergencyContactPhone;
            student.EmergencyContactRelation = dto.EmergencyContactRelation;
            student.EmergencyContactAddress = dto.EmergencyContactAddress;
            student.AvatarUrl = dto.AvatarUrl;
            student.IsActive = dto.IsActive;
            student.Notes = dto.Notes;
            student.UserId = dto.UserId;
            student.ProfileCompletionPct = CalculateProfileCompletion(dto);

            await _studentRepository.UpdateAsync(student);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var student = await _studentRepository.GetByIdAsync(id);
            if (student == null)
                return NotFound(new { message = "Không tìm thấy sinh viên" });

            if (student.Contracts.Any())
                return Conflict(new { message = "Không thể xóa sinh viên vì đã có hợp đồng" });

            await _studentRepository.DeleteAsync(student);

            return NoContent();
        }

        private static StudentDto MapToDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                StudentCode = student.StudentCode,
                Faculty = student.Faculty,
                Major = student.Major,
                AcademicYear = student.AcademicYear,
                ClassCode = student.ClassCode,
                FullName = student.FullName,
                Gender = student.Gender,
                DateOfBirth = student.DateOfBirth,
                PlaceOfBirth = student.PlaceOfBirth,
                Ethnicity = student.Ethnicity,
                Religion = student.Religion,
                Nationality = student.Nationality,
                BloodType = student.BloodType,
                HealthInsuranceNumber = student.HealthInsuranceNumber,
                Phone = student.Phone,
                Email = student.Email,
                IdentityCard = student.IdentityCard,
                IdentityCardIssuedDate = student.IdentityCardIssuedDate,
                IdentityCardIssuedPlace = student.IdentityCardIssuedPlace,
                PermanentAddress = student.PermanentAddress,
                PermanentProvince = student.PermanentProvince,
                TemporaryAddress = student.TemporaryAddress,
                EmergencyContactName = student.EmergencyContactName,
                EmergencyContactPhone = student.EmergencyContactPhone,
                EmergencyContactRelation = student.EmergencyContactRelation,
                EmergencyContactAddress = student.EmergencyContactAddress,
                AvatarUrl = student.AvatarUrl,
                ProfileCompletionPct = student.ProfileCompletionPct,
                IsActive = student.IsActive,
                Notes = student.Notes,
                UserId = student.UserId
            };
        }

        private static int CalculateProfileCompletion(CreateStudentDto dto)
        {
            int total = 0;
            int filled = 0;

            // Required fields (always count)
            total += 10;
            filled += 10;

            // Optional fields
            if (!string.IsNullOrWhiteSpace(dto.ClassCode)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.PlaceOfBirth)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.Ethnicity)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.Religion)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.BloodType)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.HealthInsuranceNumber)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.PermanentProvince)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.TemporaryAddress)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.EmergencyContactAddress)) { total++; filled++; } else total++;
            if (!string.IsNullOrWhiteSpace(dto.AvatarUrl)) { total++; filled++; } else total++;

            return (int)((filled / (double)total) * 100);
        }
    }
}
