using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomRepository _roomRepository;
        private readonly IFloorRepository _floorRepository;

        public BuildingsController(
            IBuildingRepository buildingRepository,
            IRoomRepository roomRepository,
            IFloorRepository floorRepository)
        {
            _buildingRepository = buildingRepository;
            _roomRepository = roomRepository;
            _floorRepository = floorRepository;
        }

        private static string BuildFloorLabel(int floorNumber)
        {
            return $"Tầng {floorNumber}";
        }

        private async Task CreateMissingFloorsAsync(Building building, ISet<int> existingFloorNumbers)
        {
            for (var floorNumber = 1; floorNumber <= building.TotalFloors; floorNumber++)
            {
                if (!existingFloorNumbers.Contains(floorNumber))
                {
                    var floor = new Floor
                    {
                        BuildingId = building.Id,
                        FloorNumber = floorNumber,
                        Label = BuildFloorLabel(floorNumber),
                        TotalRooms = 0,
                        IsActive = true
                    };
                    await _floorRepository.AddAsync(floor);
                }
            }
        }

        private async Task DeleteExtraFloorsAsync(Building building)
        {
            var floors = await _floorRepository.GetByBuildingIdAsync(building.Id);
            var extraFloors = floors.Where(f => f.FloorNumber > building.TotalFloors).ToList();
            foreach (var floor in extraFloors)
            {
                await _floorRepository.DeleteAsync(floor);
            }
        }

        private bool HasRoomsOnExtraFloors(IEnumerable<Room> rooms, int totalFloors)
        {
            return rooms.Any(r => r.Floor.FloorNumber > totalFloors);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BuildingDto>>> GetAll()
        {
            var buildings = await _buildingRepository.GetAllAsync();
            var buildingDtos = buildings.Select(b => new BuildingDto
            {
                Id = b.Id,
                Name = b.Name,
                Gender = b.Gender,
                TotalFloors = b.TotalFloors,
                TotalRooms = b.TotalRooms,
                Address = b.Address,
                Description = b.Description,
                ManagerName = b.ManagerName,
                ManagerPhone = b.ManagerPhone,
                ConstructionYear = b.ConstructionYear,
                Status = b.Status,
                HasElevator = b.HasElevator,
                HasParking = b.HasParking,
                HasLaundry = b.HasLaundry,
                ThumbnailUrl = b.ThumbnailUrl
            });

            return Ok(buildingDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BuildingDto>> GetById(int id)
        {
            var building = await _buildingRepository.GetByIdAsync(id);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            var buildingDto = new BuildingDto
            {
                Id = building.Id,
                Name = building.Name,
                Gender = building.Gender,
                TotalFloors = building.TotalFloors,
                TotalRooms = building.TotalRooms,
                Address = building.Address,
                Description = building.Description,
                ManagerName = building.ManagerName,
                ManagerPhone = building.ManagerPhone,
                ConstructionYear = building.ConstructionYear,
                Status = building.Status,
                HasElevator = building.HasElevator,
                HasParking = building.HasParking,
                HasLaundry = building.HasLaundry,
                ThumbnailUrl = building.ThumbnailUrl
            };

            return Ok(buildingDto);
        }

        [HttpPost]
        public async Task<ActionResult<BuildingDto>> Create([FromBody] CreateBuildingDto dto)
        {
            if (dto.TotalFloors < 1)
                return BadRequest(new { message = "Số tầng phải lớn hơn hoặc bằng 1" });

            var building = new Building
            {
                Name = dto.Name,
                Gender = dto.Gender,
                TotalFloors = dto.TotalFloors,
                TotalRooms = dto.TotalRooms,
                Address = dto.Address,
                Description = dto.Description,
                ManagerName = dto.ManagerName,
                ManagerPhone = dto.ManagerPhone,
                ConstructionYear = dto.ConstructionYear,
                Status = dto.Status,
                HasElevator = dto.HasElevator,
                HasParking = dto.HasParking,
                HasLaundry = dto.HasLaundry,
                IsActive = dto.IsActive,
                ThumbnailUrl = dto.ThumbnailUrl
            };

            await _buildingRepository.AddAsync(building);

            var existingFloors = (await _floorRepository.GetByBuildingIdAsync(building.Id)).Select(f => f.FloorNumber).ToHashSet();
            await CreateMissingFloorsAsync(building, existingFloors);

            var buildingDto = new BuildingDto
            {
                Id = building.Id,
                Name = building.Name,
                Gender = building.Gender,
                TotalFloors = building.TotalFloors,
                TotalRooms = building.TotalRooms,
                Address = building.Address,
                Description = building.Description,
                ManagerName = building.ManagerName,
                ManagerPhone = building.ManagerPhone,
                ConstructionYear = building.ConstructionYear,
                Status = building.Status,
                HasElevator = building.HasElevator,
                HasParking = building.HasParking,
                HasLaundry = building.HasLaundry,
                ThumbnailUrl = building.ThumbnailUrl
            };

            return CreatedAtAction(nameof(GetById), new { id = building.Id }, buildingDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateBuildingDto dto)
        {
            if (dto.TotalFloors < 1)
                return BadRequest(new { message = "Số tầng phải lớn hơn hoặc bằng 1" });

            var building = await _buildingRepository.GetByIdAsync(id);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            var existingFloors = (await _floorRepository.GetByBuildingIdAsync(id)).ToList();
            var rooms = await _roomRepository.GetByBuildingIdAsync(id);
            if (existingFloors.Any(f => f.FloorNumber > dto.TotalFloors) && rooms.Any(r => r.Floor.FloorNumber > dto.TotalFloors))
            {
                return Conflict(new { message = "Không thể giảm số tầng vì vẫn có phòng trên các tầng vượt quá số tầng mới" });
            }

            building.Name = dto.Name;
            building.Gender = dto.Gender;
            building.TotalFloors = dto.TotalFloors;
            building.TotalRooms = dto.TotalRooms;
            building.Address = dto.Address;
            building.Description = dto.Description;
            building.ManagerName = dto.ManagerName;
            building.ManagerPhone = dto.ManagerPhone;
            building.ConstructionYear = dto.ConstructionYear;
            building.Status = dto.Status;
            building.HasElevator = dto.HasElevator;
            building.HasParking = dto.HasParking;
            building.HasLaundry = dto.HasLaundry;
            building.IsActive = dto.IsActive;
            building.ThumbnailUrl = dto.ThumbnailUrl;

            await _buildingRepository.UpdateAsync(building);

            var existingFloorNumbers = existingFloors.Select(f => f.FloorNumber).ToHashSet();
            await CreateMissingFloorsAsync(building, existingFloorNumbers);
            await DeleteExtraFloorsAsync(building);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var building = await _buildingRepository.GetByIdAsync(id);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            var rooms = await _roomRepository.GetByBuildingIdAsync(id);
            if (rooms.Any())
                return Conflict(new { message = "Không thể xóa tòa nhà vì vẫn còn phòng thuộc tòa nhà này" });

            await _buildingRepository.DeleteAsync(building);

            return NoContent();
        }
    }

    public class CreateBuildingDto
    {
        public string Name { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int TotalFloors { get; set; }
        public int TotalRooms { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
        public string? ManagerName { get; set; }
        public string? ManagerPhone { get; set; }
        public string? ConstructionYear { get; set; }
        public string Status { get; set; } = "Active";
        public bool HasElevator { get; set; } = false;
        public bool HasParking { get; set; } = false;
        public bool HasLaundry { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? ThumbnailUrl { get; set; }
    }
}
