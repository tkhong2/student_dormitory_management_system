using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingAnnouncementsController : ControllerBase
    {
        private readonly IBuildingAnnouncementRepository _announcementRepository;
        private readonly IBuildingRepository _buildingRepository;

        public BuildingAnnouncementsController(
            IBuildingAnnouncementRepository announcementRepository,
            IBuildingRepository buildingRepository)
        {
            _announcementRepository = announcementRepository;
            _buildingRepository = buildingRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildingAnnouncementDto>>> GetAll()
        {
            var announcements = await _announcementRepository.GetAllAsync();
            var announcementDtos = announcements.Select(a => new BuildingAnnouncementDto
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                BuildingName = a.Building?.Name,
                Title = a.Title,
                Content = a.Content,
                Category = a.Category,
                Priority = a.Priority,
                IsPinned = a.IsPinned,
                PublishedAt = a.PublishedAt,
                ExpiredAt = a.ExpiredAt,
                CreatedByUserId = a.CreatedByUserId,
                CreatedByName = a.CreatedByName,
                ImageUrl = a.ImageUrl,
                CreatedAt = a.CreatedAt
            });

            return Ok(announcementDtos);
        }

        [HttpGet("published")]
        public async Task<ActionResult<IEnumerable<BuildingAnnouncementDto>>> GetPublished()
        {
            var announcements = await _announcementRepository.GetPublishedAsync();
            var announcementDtos = announcements.Select(a => new BuildingAnnouncementDto
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                BuildingName = a.Building?.Name,
                Title = a.Title,
                Content = a.Content,
                Category = a.Category,
                Priority = a.Priority,
                IsPinned = a.IsPinned,
                PublishedAt = a.PublishedAt,
                ExpiredAt = a.ExpiredAt,
                CreatedByUserId = a.CreatedByUserId,
                CreatedByName = a.CreatedByName,
                ImageUrl = a.ImageUrl,
                CreatedAt = a.CreatedAt
            });

            return Ok(announcementDtos);
        }

        [HttpGet("building/{buildingId}")]
        public async Task<ActionResult<IEnumerable<BuildingAnnouncementDto>>> GetByBuildingId(int? buildingId)
        {
            if (buildingId.HasValue)
            {
                var building = await _buildingRepository.GetByIdAsync(buildingId.Value);
                if (building == null)
                    return NotFound(new { message = "Không tìm thấy tòa nhà" });
            }

            var announcements = await _announcementRepository.GetByBuildingIdAsync(buildingId);
            var announcementDtos = announcements.Select(a => new BuildingAnnouncementDto
            {
                Id = a.Id,
                BuildingId = a.BuildingId,
                BuildingName = a.Building?.Name,
                Title = a.Title,
                Content = a.Content,
                Category = a.Category,
                Priority = a.Priority,
                IsPinned = a.IsPinned,
                PublishedAt = a.PublishedAt,
                ExpiredAt = a.ExpiredAt,
                CreatedByUserId = a.CreatedByUserId,
                CreatedByName = a.CreatedByName,
                ImageUrl = a.ImageUrl,
                CreatedAt = a.CreatedAt
            });

            return Ok(announcementDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuildingAnnouncementDto>> GetById(int id)
        {
            var announcement = await _announcementRepository.GetByIdAsync(id);
            if (announcement == null)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            var announcementDto = new BuildingAnnouncementDto
            {
                Id = announcement.Id,
                BuildingId = announcement.BuildingId,
                BuildingName = announcement.Building?.Name,
                Title = announcement.Title,
                Content = announcement.Content,
                Category = announcement.Category,
                Priority = announcement.Priority,
                IsPinned = announcement.IsPinned,
                PublishedAt = announcement.PublishedAt,
                ExpiredAt = announcement.ExpiredAt,
                CreatedByUserId = announcement.CreatedByUserId,
                CreatedByName = announcement.CreatedByName,
                ImageUrl = announcement.ImageUrl,
                CreatedAt = announcement.CreatedAt
            };

            return Ok(announcementDto);
        }

        [HttpPost]
        public async Task<ActionResult<BuildingAnnouncementDto>> Create([FromBody] CreateBuildingAnnouncementDto dto)
        {
            if (dto.BuildingId.HasValue)
            {
                var building = await _buildingRepository.GetByIdAsync(dto.BuildingId.Value);
                if (building == null)
                    return BadRequest(new { message = "Tòa nhà không tồn tại" });
            }

            var announcement = new BuildingAnnouncement
            {
                BuildingId = dto.BuildingId,
                Title = dto.Title,
                Content = dto.Content,
                Category = dto.Category,
                Priority = dto.Priority,
                IsPinned = dto.IsPinned,
                PublishedAt = dto.PublishedAt,
                ExpiredAt = dto.ExpiredAt,
                CreatedByUserId = dto.CreatedByUserId,
                CreatedByName = dto.CreatedByName,
                ImageUrl = dto.ImageUrl
            };

            await _announcementRepository.AddAsync(announcement);

            var announcementDto = new BuildingAnnouncementDto
            {
                Id = announcement.Id,
                BuildingId = announcement.BuildingId,
                Title = announcement.Title,
                Content = announcement.Content,
                Category = announcement.Category,
                Priority = announcement.Priority,
                IsPinned = announcement.IsPinned,
                PublishedAt = announcement.PublishedAt,
                ExpiredAt = announcement.ExpiredAt,
                CreatedByUserId = announcement.CreatedByUserId,
                CreatedByName = announcement.CreatedByName,
                ImageUrl = announcement.ImageUrl,
                CreatedAt = announcement.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = announcement.Id }, announcementDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateBuildingAnnouncementDto dto)
        {
            var announcement = await _announcementRepository.GetByIdAsync(id);
            if (announcement == null)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            if (dto.BuildingId.HasValue)
            {
                var building = await _buildingRepository.GetByIdAsync(dto.BuildingId.Value);
                if (building == null)
                    return BadRequest(new { message = "Tòa nhà không tồn tại" });
            }

            announcement.BuildingId = dto.BuildingId;
            announcement.Title = dto.Title;
            announcement.Content = dto.Content;
            announcement.Category = dto.Category;
            announcement.Priority = dto.Priority;
            announcement.IsPinned = dto.IsPinned;
            announcement.PublishedAt = dto.PublishedAt;
            announcement.ExpiredAt = dto.ExpiredAt;
            announcement.ImageUrl = dto.ImageUrl;

            await _announcementRepository.UpdateAsync(announcement);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var announcement = await _announcementRepository.GetByIdAsync(id);
            if (announcement == null)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            await _announcementRepository.DeleteAsync(announcement);

            return NoContent();
        }
    }
}
