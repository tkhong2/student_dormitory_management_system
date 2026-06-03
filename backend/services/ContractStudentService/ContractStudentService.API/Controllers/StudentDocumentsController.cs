using Microsoft.AspNetCore.Mvc;
using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StudentDocumentsController : ControllerBase
    {
        private readonly IStudentDocumentRepository _documentRepository;
        private readonly IStudentRepository _studentRepository;

        public StudentDocumentsController(
            IStudentDocumentRepository documentRepository,
            IStudentRepository studentRepository)
        {
            _documentRepository = documentRepository;
            _studentRepository = studentRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDocumentDto>>> GetAll()
        {
            var documents = await _documentRepository.GetAllAsync();
            var dtos = documents.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("student/{studentId}")]
        public async Task<ActionResult<IEnumerable<StudentDocumentDto>>> GetByStudentId(int studentId)
        {
            var documents = await _documentRepository.GetByStudentIdAsync(studentId);
            var dtos = documents.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StudentDocumentDto>> GetById(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound(new { message = "Không tìm thấy giấy tờ" });

            return Ok(MapToDto(document));
        }

        [HttpPost]
        public async Task<ActionResult<StudentDocumentDto>> Create([FromBody] CreateStudentDocumentDto dto)
        {
            var student = await _studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
                return BadRequest(new { message = "Sinh viên không tồn tại" });

            var document = new StudentDocument
            {
                StudentId = dto.StudentId,
                DocumentType = dto.DocumentType,
                DocumentName = dto.DocumentName,
                FileUrl = dto.FileUrl,
                FileType = dto.FileType,
                FileSizeBytes = dto.FileSizeBytes,
                Notes = dto.Notes,
                IsVerified = false
            };

            await _documentRepository.AddAsync(document);

            return CreatedAtAction(nameof(GetById), new { id = document.Id }, MapToDto(document));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateStudentDocumentDto dto)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound(new { message = "Không tìm thấy giấy tờ" });

            document.DocumentType = dto.DocumentType;
            document.DocumentName = dto.DocumentName;
            document.FileUrl = dto.FileUrl;
            document.FileType = dto.FileType;
            document.FileSizeBytes = dto.FileSizeBytes;
            document.Notes = dto.Notes;

            await _documentRepository.UpdateAsync(document);

            return NoContent();
        }

        [HttpPut("{id}/verify")]
        public async Task<ActionResult> Verify(int id, [FromBody] VerifyDocumentRequest request)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound(new { message = "Không tìm thấy giấy tờ" });

            document.IsVerified = true;
            document.VerifiedByUserId = request.VerifiedByUserId;
            document.VerifiedAt = DateTime.UtcNow;

            await _documentRepository.UpdateAsync(document);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var document = await _documentRepository.GetByIdAsync(id);
            if (document == null)
                return NotFound(new { message = "Không tìm thấy giấy tờ" });

            await _documentRepository.DeleteAsync(document);

            return NoContent();
        }

        private static StudentDocumentDto MapToDto(StudentDocument doc)
        {
            return new StudentDocumentDto
            {
                Id = doc.Id,
                StudentId = doc.StudentId,
                StudentName = doc.Student?.FullName,
                StudentCode = doc.Student?.StudentCode,
                DocumentType = doc.DocumentType,
                DocumentName = doc.DocumentName,
                FileUrl = doc.FileUrl,
                FileType = doc.FileType,
                FileSizeBytes = doc.FileSizeBytes,
                IsVerified = doc.IsVerified,
                VerifiedByUserId = doc.VerifiedByUserId,
                VerifiedAt = doc.VerifiedAt,
                Notes = doc.Notes,
                CreatedAt = doc.CreatedAt
            };
        }
    }

    public class VerifyDocumentRequest
    {
        public int VerifiedByUserId { get; set; }
    }
}
