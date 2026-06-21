using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomAmenityInspectionsController : ControllerBase
    {
        private readonly IRoomAmenityInspectionRepository _amenityInspectionRepository;
        private readonly IRoomInspectionRepository _roomInspectionRepository;

        public RoomAmenityInspectionsController(
            IRoomAmenityInspectionRepository amenityInspectionRepository,
            IRoomInspectionRepository roomInspectionRepository)
        {
            _amenityInspectionRepository = amenityInspectionRepository;
            _roomInspectionRepository = roomInspectionRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomAmenityInspectionDto>>> GetAll()
        {
            var inspections = await _amenityInspectionRepository.GetAllAsync();
            return Ok(inspections.Select(MapToDto));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomAmenityInspectionDto>> GetById(int id)
        {
            var inspection = await _amenityInspectionRepository.GetByIdAsync(id);
            if (inspection == null)
                return NotFound(new { message = "Không tìm thấy kiểm tra tiện nghi" });

            return Ok(MapToDto(inspection));
        }

        [HttpGet("room-inspection/{roomInspectionId}")]
        public async Task<ActionResult<IEnumerable<RoomAmenityInspectionDto>>> GetByRoomInspectionId(int roomInspectionId)
        {
            var inspections = await _amenityInspectionRepository.GetByRoomInspectionIdAsync(roomInspectionId);
            return Ok(inspections.Select(MapToDto));
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<RoomAmenityInspectionDto>>> GetByRoomId(int roomId)
        {
            var inspections = await _amenityInspectionRepository.GetByRoomIdAsync(roomId);
            return Ok(inspections.Select(MapToDto));
        }

        [HttpGet("need-maintenance")]
        public async Task<ActionResult<IEnumerable<RoomAmenityInspectionDto>>> GetNeedMaintenance()
        {
            var inspections = await _amenityInspectionRepository.GetNeedMaintenanceAsync();
            return Ok(inspections.Select(MapToDto));
        }

        [HttpPost]
        public async Task<ActionResult<RoomAmenityInspectionDto>> Create([FromBody] CreateRoomAmenityInspectionDto dto)
        {
            var roomInspection = await _roomInspectionRepository.GetByIdAsync(dto.RoomInspectionId);
            if (roomInspection == null)
                return BadRequest(new { message = "Biên bản kiểm tra phòng không tồn tại" });

            var inspection = new RoomAmenityInspection
            {
                RoomInspectionId = dto.RoomInspectionId,
                RoomId = dto.RoomId,
                RoomNumber = dto.RoomNumber,
                AmenityId = dto.AmenityId,
                AmenityName = dto.AmenityName,
                AmenityCategory = dto.AmenityCategory,
                Condition = dto.Condition,
                IssueDescription = dto.IssueDescription,
                RecommendedAction = dto.RecommendedAction,
                ImageUrls = dto.ImageUrls,
                NeedMaintenance = dto.NeedMaintenance,
                Priority = dto.Priority,
                Notes = dto.Notes
            };

            await _amenityInspectionRepository.AddAsync(inspection);

            return CreatedAtAction(nameof(GetById), new { id = inspection.Id }, MapToDto(inspection));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateRoomAmenityInspectionDto dto)
        {
            var inspection = await _amenityInspectionRepository.GetByIdAsync(id);
            if (inspection == null)
                return NotFound(new { message = "Không tìm thấy kiểm tra tiện nghi" });

            inspection.Condition = dto.Condition;
            inspection.IssueDescription = dto.IssueDescription;
            inspection.RecommendedAction = dto.RecommendedAction;
            inspection.ImageUrls = dto.ImageUrls;
            inspection.NeedMaintenance = dto.NeedMaintenance;
            inspection.Priority = dto.Priority;
            inspection.Notes = dto.Notes;

            await _amenityInspectionRepository.UpdateAsync(inspection);

            return NoContent();
        }

        [HttpPost("{id}/link-maintenance-request")]
        public async Task<ActionResult> LinkMaintenanceRequest(int id, [FromBody] LinkMaintenanceRequestDto dto)
        {
            var inspection = await _amenityInspectionRepository.GetByIdAsync(id);
            if (inspection == null)
                return NotFound(new { message = "Không tìm thấy kiểm tra tiện nghi" });

            inspection.MaintenanceRequestId = dto.MaintenanceRequestId;
            inspection.MaintenanceRequestCreated = true;

            await _amenityInspectionRepository.UpdateAsync(inspection);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var inspection = await _amenityInspectionRepository.GetByIdAsync(id);
            if (inspection == null)
                return NotFound(new { message = "Không tìm thấy kiểm tra tiện nghi" });

            await _amenityInspectionRepository.DeleteAsync(inspection);

            return NoContent();
        }

        private static RoomAmenityInspectionDto MapToDto(RoomAmenityInspection inspection)
        {
            return new RoomAmenityInspectionDto
            {
                Id = inspection.Id,
                RoomInspectionId = inspection.RoomInspectionId,
                RoomId = inspection.RoomId,
                RoomNumber = inspection.RoomNumber,
                AmenityId = inspection.AmenityId,
                AmenityName = inspection.AmenityName,
                AmenityCategory = inspection.AmenityCategory,
                Condition = inspection.Condition,
                IssueDescription = inspection.IssueDescription,
                RecommendedAction = inspection.RecommendedAction,
                ImageUrls = inspection.ImageUrls,
                MaintenanceRequestId = inspection.MaintenanceRequestId,
                NeedMaintenance = inspection.NeedMaintenance,
                MaintenanceRequestCreated = inspection.MaintenanceRequestCreated,
                Priority = inspection.Priority,
                Notes = inspection.Notes,
                CreatedAt = inspection.CreatedAt
            };
        }
    }

    public class LinkMaintenanceRequestDto
    {
        public int MaintenanceRequestId { get; set; }
    }
}
