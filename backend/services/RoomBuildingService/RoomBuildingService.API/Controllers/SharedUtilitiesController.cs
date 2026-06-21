using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SharedUtilitiesController : ControllerBase
    {
        private readonly ISharedUtilityRepository _repository;
        private readonly IBuildingRepository _buildingRepository;

        public SharedUtilitiesController(ISharedUtilityRepository repository, IBuildingRepository buildingRepository)
        {
            _repository = repository;
            _buildingRepository = buildingRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SharedUtilityDto>>> GetAll()
        {
            var utilities = await _repository.GetAllAsync();
            return Ok(utilities.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<SharedUtilityDto>> GetById(int id)
        {
            var utility = await _repository.GetByIdAsync(id);
            if (utility == null) return NotFound();
            return Ok(MapToDto(utility));
        }

        [HttpGet("building/{buildingId}")]
        public async Task<ActionResult<IEnumerable<SharedUtilityDto>>> GetByBuilding(int buildingId)
        {
            var utilities = await _repository.GetByBuildingIdAsync(buildingId);
            return Ok(utilities.Select(MapToDto));
        }

        [HttpGet("category/{category}")]
        public async Task<ActionResult<IEnumerable<SharedUtilityDto>>> GetByCategory(string category)
        {
            var utilities = await _repository.GetByCategoryAsync(category);
            return Ok(utilities.Select(MapToDto));
        }

        [HttpGet("status/{status}")]
        public async Task<ActionResult<IEnumerable<SharedUtilityDto>>> GetByStatus(string status)
        {
            var utilities = await _repository.GetByStatusAsync(status);
            return Ok(utilities.Select(MapToDto));
        }

        [HttpGet("utilityid/{utilityId}")]
        public async Task<ActionResult<SharedUtilityDto>> GetByUtilityId(string utilityId)
        {
            var utility = await _repository.GetByUtilityIdAsync(utilityId);
            if (utility == null) return NotFound();
            return Ok(MapToDto(utility));
        }

        [HttpPost]
        public async Task<ActionResult<SharedUtilityDto>> Create([FromBody] CreateSharedUtilityDto dto)
        {
            // Check if UtilityId already exists
            var existing = await _repository.GetByUtilityIdAsync(dto.UtilityId);
            if (existing != null)
                return BadRequest(new { message = "UtilityId đã tồn tại" });

            var utility = new SharedUtility
            {
                BuildingId = dto.BuildingId,
                Name = dto.Name,
                Category = dto.Category,
                Brand = dto.Brand,
                UtilityId = dto.UtilityId,
                Status = dto.Status,
                Location = dto.Location,
                ManagerName = dto.ManagerName,
                ManagerPhone = dto.ManagerPhone,
                ManagerEmail = dto.ManagerEmail,
                SocialLinks = dto.SocialLinks,
                Description = dto.Description,
                ImageUrl = dto.ImageUrl,
                FeePerUse = dto.FeePerUse,
                OperatingHours = dto.OperatingHours,
                UsageInstructions = dto.UsageInstructions,
                PurchaseDate = dto.PurchaseDate,
                InstallationDate = dto.InstallationDate,
                WarrantyMonths = dto.WarrantyMonths,
                WarrantyExpiry = dto.WarrantyMonths.HasValue && dto.PurchaseDate.HasValue 
                    ? dto.PurchaseDate.Value.AddMonths(dto.WarrantyMonths.Value) 
                    : null
            };

            await _repository.AddAsync(utility);
            return CreatedAtAction(nameof(GetById), new { id = utility.Id }, MapToDto(utility));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateSharedUtilityDto dto)
        {
            var utility = await _repository.GetByIdAsync(id);
            if (utility == null) return NotFound();

            if (dto.Name != null) utility.Name = dto.Name;
            if (dto.Category != null) utility.Category = dto.Category;
            if (dto.Brand != null) utility.Brand = dto.Brand;
            if (dto.Status != null) utility.Status = dto.Status;
            if (dto.Location != null) utility.Location = dto.Location;
            if (dto.ManagerName != null) utility.ManagerName = dto.ManagerName;
            if (dto.ManagerPhone != null) utility.ManagerPhone = dto.ManagerPhone;
            if (dto.ManagerEmail != null) utility.ManagerEmail = dto.ManagerEmail;
            if (dto.SocialLinks != null) utility.SocialLinks = dto.SocialLinks;
            if (dto.Description != null) utility.Description = dto.Description;
            if (dto.ImageUrl != null) utility.ImageUrl = dto.ImageUrl;
            if (dto.FeePerUse.HasValue) utility.FeePerUse = dto.FeePerUse;
            if (dto.OperatingHours != null) utility.OperatingHours = dto.OperatingHours;
            if (dto.UsageInstructions != null) utility.UsageInstructions = dto.UsageInstructions;
            if (dto.LastMaintenanceDate.HasValue) utility.LastMaintenanceDate = dto.LastMaintenanceDate;
            if (dto.NextMaintenanceDate.HasValue) utility.NextMaintenanceDate = dto.NextMaintenanceDate;

            await _repository.UpdateAsync(utility);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var utility = await _repository.GetByIdAsync(id);
            if (utility == null) return NotFound();

            await _repository.DeleteAsync(utility);
            return NoContent();
        }

        [HttpPost("{id}/use")]
        public async Task<ActionResult> RecordUsage(int id)
        {
            var utility = await _repository.GetByIdAsync(id);
            if (utility == null) return NotFound();

            utility.TotalUsageCount++;
            utility.LastUsedAt = DateTime.UtcNow;
            utility.Status = "InUse";

            await _repository.UpdateAsync(utility);
            return NoContent();
        }

        [HttpPost("{id}/complete-use")]
        public async Task<ActionResult> CompleteUsage(int id)
        {
            var utility = await _repository.GetByIdAsync(id);
            if (utility == null) return NotFound();

            utility.Status = "Available";
            await _repository.UpdateAsync(utility);
            return NoContent();
        }

        private SharedUtilityDto MapToDto(SharedUtility utility)
        {
            return new SharedUtilityDto
            {
                Id = utility.Id,
                BuildingId = utility.BuildingId,
                BuildingName = utility.Building?.Name,
                Name = utility.Name,
                Category = utility.Category,
                Brand = utility.Brand,
                UtilityId = utility.UtilityId,
                Status = utility.Status,
                Location = utility.Location,
                ManagerName = utility.ManagerName,
                ManagerPhone = utility.ManagerPhone,
                ManagerEmail = utility.ManagerEmail,
                SocialLinks = utility.SocialLinks,
                Description = utility.Description,
                ImageUrl = utility.ImageUrl,
                FeePerUse = utility.FeePerUse,
                OperatingHours = utility.OperatingHours,
                UsageInstructions = utility.UsageInstructions,
                PurchaseDate = utility.PurchaseDate,
                InstallationDate = utility.InstallationDate,
                LastMaintenanceDate = utility.LastMaintenanceDate,
                NextMaintenanceDate = utility.NextMaintenanceDate,
                WarrantyMonths = utility.WarrantyMonths,
                WarrantyExpiry = utility.WarrantyExpiry,
                TotalUsageCount = utility.TotalUsageCount,
                LastUsedAt = utility.LastUsedAt,
                CreatedAt = utility.CreatedAt
            };
        }
    }
}
