using Microsoft.AspNetCore.Mvc;
using ContractStudentService.Application.DTOs;
using ContractStudentService.Application.Interfaces;
using ContractStudentService.Domain.Entities;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContractTemplatesController : ControllerBase
    {
        private readonly IContractTemplateRepository _templateRepository;
        private readonly IContractTermRepository _termRepository;

        public ContractTemplatesController(
            IContractTemplateRepository templateRepository,
            IContractTermRepository termRepository)
        {
            _templateRepository = templateRepository;
            _termRepository = termRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContractTemplateDto>>> GetAll()
        {
            var templates = await _templateRepository.GetAllAsync();
            var templateDtos = templates.Select(MapToDto);
            return Ok(templateDtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<ContractTemplateDto>>> GetActive()
        {
            var templates = await _templateRepository.GetActiveAsync();
            var templateDtos = templates.Select(MapToDto);
            return Ok(templateDtos);
        }

        [HttpGet("default")]
        public async Task<ActionResult<ContractTemplateDto>> GetDefault()
        {
            var template = await _templateRepository.GetDefaultAsync();
            if (template == null)
                return NotFound(new { message = "Không tìm thấy mẫu hợp đồng mặc định" });

            return Ok(MapToDto(template));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ContractTemplateDto>> GetById(int id)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
                return NotFound(new { message = "Không tìm thấy mẫu hợp đồng" });

            return Ok(MapToDto(template));
        }

        [HttpGet("code/{code}")]
        public async Task<ActionResult<ContractTemplateDto>> GetByCode(string code)
        {
            var template = await _templateRepository.GetByCodeAsync(code);
            if (template == null)
                return NotFound(new { message = "Không tìm thấy mẫu hợp đồng" });

            return Ok(MapToDto(template));
        }

        [HttpGet("{id}/terms")]
        public async Task<ActionResult<IEnumerable<ContractTermDto>>> GetTerms(int id)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
                return NotFound(new { message = "Không tìm thấy mẫu hợp đồng" });

            var terms = await _termRepository.GetByTemplateIdAsync(id);
            var termDtos = terms.Select(t => new ContractTermDto
            {
                Id = t.Id,
                ContractTemplateId = t.ContractTemplateId,
                Title = t.Title,
                Content = t.Content,
                OrderIndex = t.OrderIndex,
                IsRequired = t.IsRequired,
                IsHighlighted = t.IsHighlighted,
                Icon = t.Icon
            });

            return Ok(termDtos);
        }

        [HttpPost]
        public async Task<ActionResult<ContractTemplateDto>> Create([FromBody] CreateContractTemplateDto dto)
        {
            // Check if code already exists
            var existing = await _templateRepository.GetByCodeAsync(dto.Code);
            if (existing != null)
                return Conflict(new { message = "Mã mẫu hợp đồng đã tồn tại" });

            var template = new ContractTemplate
            {
                Name = dto.Name,
                Code = dto.Code,
                Description = dto.Description,
                IsDefault = dto.IsDefault,
                IsActive = dto.IsActive,
                MinDurationMonths = dto.MinDurationMonths,
                MaxDurationMonths = dto.MaxDurationMonths,
                TemplateContent = dto.TemplateContent
            };

            await _templateRepository.AddAsync(template);

            // Add terms
            if (dto.Terms != null && dto.Terms.Any())
            {
                foreach (var termDto in dto.Terms)
                {
                    var term = new ContractTerm
                    {
                        ContractTemplateId = template.Id,
                        Title = termDto.Title,
                        Content = termDto.Content,
                        OrderIndex = termDto.OrderIndex,
                        IsRequired = termDto.IsRequired,
                        IsHighlighted = termDto.IsHighlighted,
                        Icon = termDto.Icon
                    };
                    await _termRepository.AddAsync(term);
                }
            }

            // Reload to get terms
            template = await _templateRepository.GetByIdAsync(template.Id);
            return CreatedAtAction(nameof(GetById), new { id = template!.Id }, MapToDto(template));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateContractTemplateDto dto)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
                return NotFound(new { message = "Không tìm thấy mẫu hợp đồng" });

            // Check if code conflicts with another template
            var existing = await _templateRepository.GetByCodeAsync(dto.Code);
            if (existing != null && existing.Id != id)
                return Conflict(new { message = "Mã mẫu hợp đồng đã tồn tại" });

            template.Name = dto.Name;
            template.Code = dto.Code;
            template.Description = dto.Description;
            template.IsDefault = dto.IsDefault;
            template.IsActive = dto.IsActive;
            template.MinDurationMonths = dto.MinDurationMonths;
            template.MaxDurationMonths = dto.MaxDurationMonths;
            template.TemplateContent = dto.TemplateContent;

            await _templateRepository.UpdateAsync(template);

            // Delete old terms and add new ones
            var oldTerms = await _termRepository.GetByTemplateIdAsync(id);
            foreach (var oldTerm in oldTerms)
            {
                await _termRepository.DeleteAsync(oldTerm);
            }

            if (dto.Terms != null && dto.Terms.Any())
            {
                foreach (var termDto in dto.Terms)
                {
                    var term = new ContractTerm
                    {
                        ContractTemplateId = template.Id,
                        Title = termDto.Title,
                        Content = termDto.Content,
                        OrderIndex = termDto.OrderIndex,
                        IsRequired = termDto.IsRequired,
                        IsHighlighted = termDto.IsHighlighted,
                        Icon = termDto.Icon
                    };
                    await _termRepository.AddAsync(term);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var template = await _templateRepository.GetByIdAsync(id);
            if (template == null)
                return NotFound(new { message = "Không tìm thấy mẫu hợp đồng" });

            // Don't allow deleting if it's the default template
            if (template.IsDefault)
                return BadRequest(new { message = "Không thể xóa mẫu hợp đồng mặc định" });

            await _templateRepository.DeleteAsync(template);
            return NoContent();
        }

        private static ContractTemplateDto MapToDto(ContractTemplate template)
        {
            return new ContractTemplateDto
            {
                Id = template.Id,
                Name = template.Name,
                Code = template.Code,
                Description = template.Description,
                IsDefault = template.IsDefault,
                IsActive = template.IsActive,
                MinDurationMonths = template.MinDurationMonths,
                MaxDurationMonths = template.MaxDurationMonths,
                TemplateContent = template.TemplateContent,
                Terms = template.Terms?.Select(t => new ContractTermDto
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
}
