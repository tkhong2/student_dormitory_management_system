using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FacilityReviewsController : ControllerBase
    {
        private readonly IFacilityReviewRepository _reviewRepository;
        private readonly IPublicFacilityRepository _facilityRepository;

        public FacilityReviewsController(
            IFacilityReviewRepository reviewRepository,
            IPublicFacilityRepository facilityRepository)
        {
            _reviewRepository = reviewRepository;
            _facilityRepository = facilityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FacilityReviewDto>>> GetAll()
        {
            var reviews = await _reviewRepository.GetAllAsync();
            return Ok(reviews.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FacilityReviewDto>> GetById(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return NotFound(new { message = "Không tìm thấy đánh giá" });

            return Ok(MapToDto(review));
        }

        [HttpGet("facility/{facilityId}")]
        public async Task<ActionResult<IEnumerable<FacilityReviewDto>>> GetByFacilityId(int facilityId)
        {
            var reviews = await _reviewRepository.GetByFacilityIdAsync(facilityId);
            return Ok(reviews.Select(MapToDto));
        }

        [HttpGet("facility/{facilityId}/approved")]
        public async Task<ActionResult<IEnumerable<FacilityReviewDto>>> GetApprovedByFacilityId(int facilityId)
        {
            var reviews = await _reviewRepository.GetApprovedByFacilityIdAsync(facilityId);
            return Ok(reviews.Select(MapToDto));
        }

        [HttpPost]
        public async Task<ActionResult<FacilityReviewDto>> Create([FromBody] CreateFacilityReviewDto dto)
        {
            var facility = await _facilityRepository.GetByIdAsync(dto.FacilityId);
            if (facility == null)
                return BadRequest(new { message = "Tiện ích không tồn tại" });

            if (dto.Rating < 1 || dto.Rating > 5)
                return BadRequest(new { message = "Đánh giá phải từ 1 đến 5 sao" });

            var review = new FacilityReview
            {
                FacilityId = dto.FacilityId,
                StudentId = dto.StudentId,
                StudentName = dto.StudentName,
                Rating = dto.Rating,
                Comment = dto.Comment,
                ImageUrl = dto.ImageUrl,
                IsApproved = true // Tự động duyệt, hoặc có thể để false để admin duyệt
            };

            await _reviewRepository.AddAsync(review);

            return CreatedAtAction(nameof(GetById), new { id = review.Id }, MapToDto(review));
        }

        [HttpPut("{id}/approve")]
        public async Task<ActionResult> Approve(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return NotFound(new { message = "Không tìm thấy đánh giá" });

            review.IsApproved = true;
            await _reviewRepository.UpdateAsync(review);

            return NoContent();
        }

        [HttpPut("{id}/reject")]
        public async Task<ActionResult> Reject(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return NotFound(new { message = "Không tìm thấy đánh giá" });

            review.IsApproved = false;
            await _reviewRepository.UpdateAsync(review);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var review = await _reviewRepository.GetByIdAsync(id);
            if (review == null)
                return NotFound(new { message = "Không tìm thấy đánh giá" });

            await _reviewRepository.DeleteAsync(review);

            return NoContent();
        }

        private static FacilityReviewDto MapToDto(FacilityReview review)
        {
            return new FacilityReviewDto
            {
                Id = review.Id,
                FacilityId = review.FacilityId,
                FacilityName = review.Facility?.Name ?? string.Empty,
                StudentId = review.StudentId,
                StudentName = review.StudentName,
                Rating = review.Rating,
                Comment = review.Comment,
                ImageUrl = review.ImageUrl,
                IsApproved = review.IsApproved,
                CreatedAt = review.CreatedAt
            };
        }
    }
}
