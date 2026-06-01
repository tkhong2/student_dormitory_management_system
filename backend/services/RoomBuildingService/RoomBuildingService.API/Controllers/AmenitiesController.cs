using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AmenitiesController : ControllerBase
    {
        private readonly IAmenityRepository _amenityRepository;

        public AmenitiesController(IAmenityRepository amenityRepository)
        {
            _amenityRepository = amenityRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AmenityDto>>> GetAll()
        {
            var amenities = await _amenityRepository.GetAllAsync();
            var amenityDtos = amenities.Select(a => new AmenityDto
            {
                Id = a.Id,
                Name = a.Name,
                Category = a.Category,
                IconUrl = a.IconUrl,
                IsActive = a.IsActive
            });

            return Ok(amenityDtos);
        }

        [HttpGet("active")]
        public async Task<ActionResult<IEnumerable<AmenityDto>>> GetActives()
        {
            var amenities = await _amenityRepository.GetActivesAsync();
            var amenityDtos = amenities.Select(a => new AmenityDto
            {
                Id = a.Id,
                Name = a.Name,
                Category = a.Category,
                IconUrl = a.IconUrl,
                IsActive = a.IsActive
            });

            return Ok(amenityDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AmenityDto>> GetById(int id)
        {
            var amenity = await _amenityRepository.GetByIdAsync(id);
            if (amenity == null)
                return NotFound(new { message = "Không tìm thấy tiện nghi" });

            var amenityDto = new AmenityDto
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Category = amenity.Category,
                IconUrl = amenity.IconUrl,
                IsActive = amenity.IsActive
            };

            return Ok(amenityDto);
        }

        [HttpPost]
        public async Task<ActionResult<AmenityDto>> Create([FromBody] CreateAmenityDto dto)
        {
            var amenity = new Amenity
            {
                Name = dto.Name,
                Category = dto.Category,
                IconUrl = dto.IconUrl,
                IsActive = dto.IsActive
            };

            await _amenityRepository.AddAsync(amenity);

            var amenityDto = new AmenityDto
            {
                Id = amenity.Id,
                Name = amenity.Name,
                Category = amenity.Category,
                IconUrl = amenity.IconUrl,
                IsActive = amenity.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = amenity.Id }, amenityDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateAmenityDto dto)
        {
            var amenity = await _amenityRepository.GetByIdAsync(id);
            if (amenity == null)
                return NotFound(new { message = "Không tìm thấy tiện nghi" });

            amenity.Name = dto.Name;
            amenity.Category = dto.Category;
            amenity.IconUrl = dto.IconUrl;
            amenity.IsActive = dto.IsActive;

            await _amenityRepository.UpdateAsync(amenity);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var amenity = await _amenityRepository.GetByIdAsync(id);
            if (amenity == null)
                return NotFound(new { message = "Không tìm thấy tiện nghi" });

            await _amenityRepository.DeleteAsync(amenity);

            return NoContent();
        }
    }
}
