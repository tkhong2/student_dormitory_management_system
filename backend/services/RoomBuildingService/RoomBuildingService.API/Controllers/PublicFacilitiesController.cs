using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublicFacilitiesController : ControllerBase
    {
        private readonly IPublicFacilityRepository _facilityRepository;
        private readonly IFacilityReviewRepository _reviewRepository;

        public PublicFacilitiesController(
            IPublicFacilityRepository facilityRepository,
            IFacilityReviewRepository reviewRepository)
        {
            _facilityRepository = facilityRepository;
            _reviewRepository = reviewRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PublicFacilityDto>>> GetAll()
        {
            var facilities = await _facilityRepository.GetAllAsync();
            return Ok(facilities.Select(MapToDto));
        }

        [HttpGet("homepage")]
        public async Task<ActionResult<IEnumerable<PublicFacilityDto>>> GetVisibleOnHomepage()
        {
            var facilities = await _facilityRepository.GetVisibleOnHomepageAsync();
            return Ok(facilities.Select(MapToDto));
        }

        [HttpGet("building/{buildingId}")]
        public async Task<ActionResult<IEnumerable<PublicFacilityDto>>> GetByBuildingId(int? buildingId)
        {
            var facilities = await _facilityRepository.GetByBuildingIdAsync(buildingId);
            return Ok(facilities.Select(MapToDto));
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<PublicFacilityDto>>> GetByCategory(string category)
        {
            var facilities = await _facilityRepository.GetByCategoryAsync(category);
            return Ok(facilities.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PublicFacilityDto>> GetById(int id)
        {
            var facility = await _facilityRepository.GetByIdAsync(id);
            if (facility == null)
                return NotFound(new { message = "Không tìm thấy tiện ích chung" });

            return Ok(MapToDto(facility));
        }

        [HttpPost]
        public async Task<ActionResult<PublicFacilityDto>> Create([FromBody] CreatePublicFacilityDto dto)
        {
            var facility = new PublicFacility
            {
                BuildingId = dto.BuildingId,
                Name = dto.Name,
                Category = dto.Category,
                BrandName = dto.BrandName,
                FacilityId = dto.FacilityId,
                Status = dto.Status,
                Description = dto.Description,
                Location = dto.Location,
                ImageUrl = dto.ImageUrl,
                ManagerName = dto.ManagerName,
                ManagerPhone = dto.ManagerPhone,
                ManagerEmail = dto.ManagerEmail,
                SocialLinks = dto.SocialLinks,
                OperatingHours = dto.OperatingHours,
                IsBookingRequired = dto.IsBookingRequired,
                MaxCapacity = dto.MaxCapacity,
                FeePerHour = dto.FeePerHour,
                FeePerSession = dto.FeePerSession,
                FeeNote = dto.FeeNote,
                EventSchedule = dto.EventSchedule,
                IsVisibleOnHomepage = dto.IsVisibleOnHomepage,
                DisplayOrder = dto.DisplayOrder,
                IsFeatured = dto.IsFeatured
            };

            await _facilityRepository.AddAsync(facility);

            return CreatedAtAction(nameof(GetById), new { id = facility.Id }, MapToDto(facility));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdatePublicFacilityDto dto)
        {
            var facility = await _facilityRepository.GetByIdAsync(id);
            if (facility == null)
                return NotFound(new { message = "Không tìm thấy tiện ích chung" });

            if (dto.Name != null) facility.Name = dto.Name;
            if (dto.Category != null) facility.Category = dto.Category;
            if (dto.BrandName != null) facility.BrandName = dto.BrandName;
            if (dto.FacilityId != null) facility.FacilityId = dto.FacilityId;
            if (dto.Status != null) facility.Status = dto.Status;
            if (dto.Description != null) facility.Description = dto.Description;
            if (dto.Location != null) facility.Location = dto.Location;
            if (dto.ImageUrl != null) facility.ImageUrl = dto.ImageUrl;
            if (dto.ManagerName != null) facility.ManagerName = dto.ManagerName;
            if (dto.ManagerPhone != null) facility.ManagerPhone = dto.ManagerPhone;
            if (dto.ManagerEmail != null) facility.ManagerEmail = dto.ManagerEmail;
            if (dto.SocialLinks != null) facility.SocialLinks = dto.SocialLinks;
            if (dto.OperatingHours != null) facility.OperatingHours = dto.OperatingHours;
            if (dto.IsBookingRequired.HasValue) facility.IsBookingRequired = dto.IsBookingRequired.Value;
            if (dto.MaxCapacity.HasValue) facility.MaxCapacity = dto.MaxCapacity;
            if (dto.FeePerHour.HasValue) facility.FeePerHour = dto.FeePerHour;
            if (dto.FeePerSession.HasValue) facility.FeePerSession = dto.FeePerSession;
            if (dto.FeeNote != null) facility.FeeNote = dto.FeeNote;
            if (dto.EventSchedule != null) facility.EventSchedule = dto.EventSchedule;
            if (dto.IsVisibleOnHomepage.HasValue) facility.IsVisibleOnHomepage = dto.IsVisibleOnHomepage.Value;
            if (dto.DisplayOrder.HasValue) facility.DisplayOrder = dto.DisplayOrder.Value;
            if (dto.IsFeatured.HasValue) facility.IsFeatured = dto.IsFeatured.Value;
            if (dto.LastMaintenanceDate.HasValue) facility.LastMaintenanceDate = dto.LastMaintenanceDate;
            if (dto.NextMaintenanceDate.HasValue) facility.NextMaintenanceDate = dto.NextMaintenanceDate;

            await _facilityRepository.UpdateAsync(facility);

            return NoContent();
        }

        [HttpPost("{id}/increment-usage")]
        public async Task<ActionResult> IncrementUsage(int id)
        {
            var facility = await _facilityRepository.GetByIdAsync(id);
            if (facility == null)
                return NotFound(new { message = "Không tìm thấy tiện ích chung" });

            facility.TotalUsageCount++;
            await _facilityRepository.UpdateAsync(facility);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var facility = await _facilityRepository.GetByIdAsync(id);
            if (facility == null)
                return NotFound(new { message = "Không tìm thấy tiện ích chung" });

            await _facilityRepository.DeleteAsync(facility);

            return NoContent();
        }

        private PublicFacilityDto MapToDto(PublicFacility facility)
        {
            var approvedReviews = facility.Reviews.Where(r => r.IsApproved).ToList();
            
            return new PublicFacilityDto
            {
                Id = facility.Id,
                BuildingId = facility.BuildingId,
                BuildingName = facility.BuildingName,
                Name = facility.Name,
                Category = facility.Category,
                BrandName = facility.BrandName,
                FacilityId = facility.FacilityId,
                Status = facility.Status,
                Description = facility.Description,
                Location = facility.Location,
                ImageUrl = facility.ImageUrl,
                ManagerName = facility.ManagerName,
                ManagerPhone = facility.ManagerPhone,
                ManagerEmail = facility.ManagerEmail,
                SocialLinks = facility.SocialLinks,
                OperatingHours = facility.OperatingHours,
                IsBookingRequired = facility.IsBookingRequired,
                MaxCapacity = facility.MaxCapacity,
                FeePerHour = facility.FeePerHour,
                FeePerSession = facility.FeePerSession,
                FeeNote = facility.FeeNote,
                EventSchedule = facility.EventSchedule,
                IsVisibleOnHomepage = facility.IsVisibleOnHomepage,
                DisplayOrder = facility.DisplayOrder,
                IsFeatured = facility.IsFeatured,
                TotalUsageCount = facility.TotalUsageCount,
                LastMaintenanceDate = facility.LastMaintenanceDate,
                NextMaintenanceDate = facility.NextMaintenanceDate,
                CreatedAt = facility.CreatedAt,
                AverageRating = approvedReviews.Any() ? approvedReviews.Average(r => r.Rating) : null,
                ReviewCount = approvedReviews.Count
            };
        }
    }
}
