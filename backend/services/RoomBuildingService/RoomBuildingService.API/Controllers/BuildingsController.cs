using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;
using RoomBuildingService.Domain.Enums;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingRepository _buildingRepository;

        public BuildingsController(IBuildingRepository buildingRepository)
        {
            _buildingRepository = buildingRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildingDto>>> GetAll()
        {
            var buildings = await _buildingRepository.GetAllAsync();
            var buildingDtos = buildings.Select(b => new BuildingDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                NumberOfFloors = b.NumberOfFloors,
                Type = b.Type.ToString(),
                ImageUrl = b.ImageUrl
            });

            return Ok(buildingDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuildingDto>> GetById(Guid id)
        {
            var building = await _buildingRepository.GetByIdAsync(id);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            var buildingDto = new BuildingDto
            {
                Id = building.Id,
                Name = building.Name,
                Description = building.Description,
                NumberOfFloors = building.NumberOfFloors,
                Type = building.Type.ToString(),
                ImageUrl = building.ImageUrl
            };

            return Ok(buildingDto);
        }

        [HttpPost]
        public async Task<ActionResult<BuildingDto>> Create([FromBody] CreateBuildingDto dto)
        {
            if (!Enum.TryParse<BuildingType>(dto.Type, out var buildingType))
                return BadRequest(new { message = "Loại tòa nhà không hợp lệ" });

            var building = new Building
            {
                Name = dto.Name,
                Description = dto.Description,
                NumberOfFloors = dto.NumberOfFloors,
                Type = buildingType,
                ImageUrl = dto.ImageUrl
            };

            await _buildingRepository.AddAsync(building);

            var buildingDto = new BuildingDto
            {
                Id = building.Id,
                Name = building.Name,
                Description = building.Description,
                NumberOfFloors = building.NumberOfFloors,
                Type = building.Type.ToString(),
                ImageUrl = building.ImageUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = building.Id }, buildingDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(Guid id, [FromBody] CreateBuildingDto dto)
        {
            var building = await _buildingRepository.GetByIdAsync(id);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            if (!Enum.TryParse<BuildingType>(dto.Type, out var buildingType))
                return BadRequest(new { message = "Loại tòa nhà không hợp lệ" });

            building.Name = dto.Name;
            building.Description = dto.Description;
            building.NumberOfFloors = dto.NumberOfFloors;
            building.Type = buildingType;
            building.ImageUrl = dto.ImageUrl;

            await _buildingRepository.UpdateAsync(building);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var building = await _buildingRepository.GetByIdAsync(id);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            await _buildingRepository.DeleteAsync(building);

            return NoContent();
        }
    }

    public class CreateBuildingDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int NumberOfFloors { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
