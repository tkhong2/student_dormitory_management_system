using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FloorsController : ControllerBase
    {
        private readonly IFloorRepository _floorRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomRepository _roomRepository;

        public FloorsController(
            IFloorRepository floorRepository,
            IBuildingRepository buildingRepository,
            IRoomRepository roomRepository)
        {
            _floorRepository = floorRepository;
            _buildingRepository = buildingRepository;
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<FloorDto>>> GetAll()
        {
            var floors = await _floorRepository.GetAllAsync();
            var dtos = floors.Select(f => new FloorDto
            {
                Id = f.Id,
                BuildingId = f.BuildingId,
                FloorNumber = f.FloorNumber,
                Label = f.Label,
                TotalRooms = f.TotalRooms,
                FloorPlanImageUrl = f.FloorPlanImageUrl,
                IsActive = f.IsActive
            });

            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<FloorDto>> GetById(int id)
        {
            var floor = await _floorRepository.GetByIdAsync(id);
            if (floor == null)
                return NotFound(new { message = "Không tìm thấy tầng" });

            return Ok(new FloorDto
            {
                Id = floor.Id,
                BuildingId = floor.BuildingId,
                FloorNumber = floor.FloorNumber,
                Label = floor.Label,
                TotalRooms = floor.TotalRooms,
                FloorPlanImageUrl = floor.FloorPlanImageUrl,
                IsActive = floor.IsActive
            });
        }

        [HttpGet("building/{buildingId}")]
        public async Task<ActionResult<IEnumerable<FloorDto>>> GetByBuildingId(int buildingId)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            var floors = await _floorRepository.GetByBuildingIdAsync(buildingId);
            
            // Chỉ lấy các tầng có FloorNumber <= TotalFloors của tòa nhà
            var dtos = floors
                .Where(f => f.FloorNumber <= building.TotalFloors)
                .Select(f => new FloorDto
                {
                    Id = f.Id,
                    BuildingId = f.BuildingId,
                    FloorNumber = f.FloorNumber,
                    Label = f.Label,
                    TotalRooms = f.TotalRooms,
                    FloorPlanImageUrl = f.FloorPlanImageUrl,
                    IsActive = f.IsActive
                });

            return Ok(dtos);
        }

        [HttpPost]
        public async Task<ActionResult<FloorDto>> Create([FromBody] CreateFloorDto dto)
        {
            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
                return BadRequest(new { message = "Tòa nhà không tồn tại" });

            var floor = new Floor
            {
                BuildingId = dto.BuildingId,
                FloorNumber = dto.FloorNumber,
                Label = dto.Label,
                TotalRooms = dto.TotalRooms,
                FloorPlanImageUrl = dto.FloorPlanImageUrl,
                IsActive = dto.IsActive
            };

            await _floorRepository.AddAsync(floor);

            return CreatedAtAction(nameof(GetById), new { id = floor.Id }, new FloorDto
            {
                Id = floor.Id,
                BuildingId = floor.BuildingId,
                FloorNumber = floor.FloorNumber,
                Label = floor.Label,
                TotalRooms = floor.TotalRooms,
                FloorPlanImageUrl = floor.FloorPlanImageUrl,
                IsActive = floor.IsActive
            });
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateFloorDto dto)
        {
            var floor = await _floorRepository.GetByIdAsync(id);
            if (floor == null)
                return NotFound(new { message = "Không tìm thấy tầng" });

            var building = await _buildingRepository.GetByIdAsync(dto.BuildingId);
            if (building == null)
                return BadRequest(new { message = "Tòa nhà không tồn tại" });

            floor.BuildingId = dto.BuildingId;
            floor.FloorNumber = dto.FloorNumber;
            floor.Label = dto.Label;
            floor.TotalRooms = dto.TotalRooms;
            floor.FloorPlanImageUrl = dto.FloorPlanImageUrl;
            floor.IsActive = dto.IsActive;

            await _floorRepository.UpdateAsync(floor);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var floor = await _floorRepository.GetByIdAsync(id);
            if (floor == null)
                return NotFound(new { message = "Không tìm thấy tầng" });

            var rooms = await _roomRepository.GetAllAsync();
            if (rooms.Any(r => r.FloorId == id))
                return Conflict(new { message = "Không thể xóa tầng vì vẫn có phòng tồn tại" });

            await _floorRepository.DeleteAsync(floor);
            return NoContent();
        }
    }

    public class CreateFloorDto
    {
        public int BuildingId { get; set; }
        public int FloorNumber { get; set; }
        public string Label { get; set; } = string.Empty;
        public int TotalRooms { get; set; }
        public string? FloorPlanImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
