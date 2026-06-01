using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomInspectionsController : ControllerBase
    {
        private readonly IRoomInspectionRepository _inspectionRepository;
        private readonly IRoomRepository _roomRepository;

        public RoomInspectionsController(
            IRoomInspectionRepository inspectionRepository,
            IRoomRepository roomRepository)
        {
            _inspectionRepository = inspectionRepository;
            _roomRepository = roomRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomInspectionDto>>> GetAll()
        {
            var inspections = await _inspectionRepository.GetAllAsync();
            var inspectionDtos = inspections.Select(i => new RoomInspectionDto
            {
                Id = i.Id,
                RoomId = i.RoomId,
                RoomNumber = i.Room.RoomNumber,
                InspectorUserId = i.InspectorUserId,
                InspectorName = i.InspectorName,
                InspectionDate = i.InspectionDate,
                InspectionType = i.InspectionType,
                OverallCondition = i.OverallCondition,
                ElectricalStatus = i.ElectricalStatus,
                PlumbingStatus = i.PlumbingStatus,
                FurnitureStatus = i.FurnitureStatus,
                WallStatus = i.WallStatus,
                FloorStatus = i.FloorStatus,
                ElectricityReading = i.ElectricityReading,
                WaterReading = i.WaterReading,
                Notes = i.Notes,
                ImageUrls = i.ImageUrls,
                NextInspectionDate = i.NextInspectionDate,
                IsApproved = i.IsApproved,
                ApprovedByUserId = i.ApprovedByUserId,
                ApprovedByName = i.ApprovedByName,
                ApprovedAt = i.ApprovedAt,
                ApprovalNote = i.ApprovalNote,
                CreatedAt = i.CreatedAt
            });

            return Ok(inspectionDtos);
        }

        [HttpGet("pending-approvals")]
        public async Task<ActionResult<IEnumerable<RoomInspectionDto>>> GetPendingApprovals()
        {
            var inspections = await _inspectionRepository.GetPendingApprovalsAsync();
            var inspectionDtos = inspections.Select(i => new RoomInspectionDto
            {
                Id = i.Id,
                RoomId = i.RoomId,
                RoomNumber = i.Room.RoomNumber,
                InspectorUserId = i.InspectorUserId,
                InspectorName = i.InspectorName,
                InspectionDate = i.InspectionDate,
                InspectionType = i.InspectionType,
                OverallCondition = i.OverallCondition,
                ElectricalStatus = i.ElectricalStatus,
                PlumbingStatus = i.PlumbingStatus,
                FurnitureStatus = i.FurnitureStatus,
                WallStatus = i.WallStatus,
                FloorStatus = i.FloorStatus,
                ElectricityReading = i.ElectricityReading,
                WaterReading = i.WaterReading,
                Notes = i.Notes,
                ImageUrls = i.ImageUrls,
                NextInspectionDate = i.NextInspectionDate,
                IsApproved = i.IsApproved,
                CreatedAt = i.CreatedAt
            });

            return Ok(inspectionDtos);
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<RoomInspectionDto>>> GetByRoomId(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var inspections = await _inspectionRepository.GetByRoomIdAsync(roomId);
            var inspectionDtos = inspections.Select(i => new RoomInspectionDto
            {
                Id = i.Id,
                RoomId = i.RoomId,
                InspectorUserId = i.InspectorUserId,
                InspectorName = i.InspectorName,
                InspectionDate = i.InspectionDate,
                InspectionType = i.InspectionType,
                OverallCondition = i.OverallCondition,
                ElectricalStatus = i.ElectricalStatus,
                PlumbingStatus = i.PlumbingStatus,
                FurnitureStatus = i.FurnitureStatus,
                WallStatus = i.WallStatus,
                FloorStatus = i.FloorStatus,
                ElectricityReading = i.ElectricityReading,
                WaterReading = i.WaterReading,
                Notes = i.Notes,
                ImageUrls = i.ImageUrls,
                NextInspectionDate = i.NextInspectionDate,
                IsApproved = i.IsApproved,
                ApprovedByUserId = i.ApprovedByUserId,
                ApprovedByName = i.ApprovedByName,
                ApprovedAt = i.ApprovedAt,
                ApprovalNote = i.ApprovalNote,
                CreatedAt = i.CreatedAt
            });

            return Ok(inspectionDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomInspectionDto>> GetById(int id)
        {
            var inspection = await _inspectionRepository.GetByIdAsync(id);
            if (inspection == null)
                return NotFound(new { message = "Không tìm thấy biên bản kiểm tra" });

            var inspectionDto = new RoomInspectionDto
            {
                Id = inspection.Id,
                RoomId = inspection.RoomId,
                RoomNumber = inspection.Room.RoomNumber,
                InspectorUserId = inspection.InspectorUserId,
                InspectorName = inspection.InspectorName,
                InspectionDate = inspection.InspectionDate,
                InspectionType = inspection.InspectionType,
                OverallCondition = inspection.OverallCondition,
                ElectricalStatus = inspection.ElectricalStatus,
                PlumbingStatus = inspection.PlumbingStatus,
                FurnitureStatus = inspection.FurnitureStatus,
                WallStatus = inspection.WallStatus,
                FloorStatus = inspection.FloorStatus,
                ElectricityReading = inspection.ElectricityReading,
                WaterReading = inspection.WaterReading,
                Notes = inspection.Notes,
                ImageUrls = inspection.ImageUrls,
                NextInspectionDate = inspection.NextInspectionDate,
                IsApproved = inspection.IsApproved,
                ApprovedByUserId = inspection.ApprovedByUserId,
                ApprovedByName = inspection.ApprovedByName,
                ApprovedAt = inspection.ApprovedAt,
                ApprovalNote = inspection.ApprovalNote,
                CreatedAt = inspection.CreatedAt
            };

            return Ok(inspectionDto);
        }

        [HttpPost]
        public async Task<ActionResult<RoomInspectionDto>> Create([FromBody] CreateRoomInspectionDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null)
                return BadRequest(new { message = "Phòng không tồn tại" });

            var inspection = new RoomInspection
            {
                RoomId = dto.RoomId,
                InspectorUserId = dto.InspectorUserId,
                InspectorName = dto.InspectorName,
                InspectionDate = dto.InspectionDate,
                InspectionType = dto.InspectionType,
                OverallCondition = dto.OverallCondition,
                ElectricalStatus = dto.ElectricalStatus,
                PlumbingStatus = dto.PlumbingStatus,
                FurnitureStatus = dto.FurnitureStatus,
                WallStatus = dto.WallStatus,
                FloorStatus = dto.FloorStatus,
                ElectricityReading = dto.ElectricityReading,
                WaterReading = dto.WaterReading,
                Notes = dto.Notes,
                ImageUrls = dto.ImageUrls,
                NextInspectionDate = dto.NextInspectionDate
            };

            await _inspectionRepository.AddAsync(inspection);

            // Update room's LastInspectedAt
            room.LastInspectedAt = DateTime.UtcNow;
            await _roomRepository.UpdateAsync(room);

            var inspectionDto = new RoomInspectionDto
            {
                Id = inspection.Id,
                RoomId = inspection.RoomId,
                InspectorUserId = inspection.InspectorUserId,
                InspectorName = inspection.InspectorName,
                InspectionDate = inspection.InspectionDate,
                InspectionType = inspection.InspectionType,
                OverallCondition = inspection.OverallCondition,
                ElectricalStatus = inspection.ElectricalStatus,
                PlumbingStatus = inspection.PlumbingStatus,
                FurnitureStatus = inspection.FurnitureStatus,
                WallStatus = inspection.WallStatus,
                FloorStatus = inspection.FloorStatus,
                ElectricityReading = inspection.ElectricityReading,
                WaterReading = inspection.WaterReading,
                Notes = inspection.Notes,
                ImageUrls = inspection.ImageUrls,
                NextInspectionDate = inspection.NextInspectionDate,
                IsApproved = inspection.IsApproved,
                CreatedAt = inspection.CreatedAt
            };

            return CreatedAtAction(nameof(GetById), new { id = inspection.Id }, inspectionDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateRoomInspectionDto dto)
        {
            var inspection = await _inspectionRepository.GetByIdAsync(id);
            if (inspection == null)
                return NotFound(new { message = "Không tìm thấy biên bản kiểm tra" });

            inspection.InspectionDate = dto.InspectionDate;
            inspection.InspectionType = dto.InspectionType;
            inspection.OverallCondition = dto.OverallCondition;
            inspection.ElectricalStatus = dto.ElectricalStatus;
            inspection.PlumbingStatus = dto.PlumbingStatus;
            inspection.FurnitureStatus = dto.FurnitureStatus;
            inspection.WallStatus = dto.WallStatus;
            inspection.FloorStatus = dto.FloorStatus;
            inspection.ElectricityReading = dto.ElectricityReading;
            inspection.WaterReading = dto.WaterReading;
            inspection.Notes = dto.Notes;
            inspection.ImageUrls = dto.ImageUrls;
            inspection.NextInspectionDate = dto.NextInspectionDate;

            await _inspectionRepository.UpdateAsync(inspection);

            return NoContent();
        }

        [HttpPost("{id}/approve")]
        public async Task<ActionResult> Approve(int id, [FromBody] ApproveInspectionDto dto)
        {
            var inspection = await _inspectionRepository.GetByIdAsync(id);
            if (inspection == null)
                return NotFound(new { message = "Không tìm thấy biên bản kiểm tra" });

            if (inspection.IsApproved)
                return BadRequest(new { message = "Biên bản đã được duyệt" });

            inspection.IsApproved = true;
            inspection.ApprovedByUserId = dto.ApprovedByUserId;
            inspection.ApprovedByName = dto.ApprovedByName;
            inspection.ApprovedAt = DateTime.UtcNow;
            inspection.ApprovalNote = dto.ApprovalNote;

            await _inspectionRepository.UpdateAsync(inspection);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var inspection = await _inspectionRepository.GetByIdAsync(id);
            if (inspection == null)
                return NotFound(new { message = "Không tìm thấy biên bản kiểm tra" });

            await _inspectionRepository.DeleteAsync(inspection);

            return NoContent();
        }
    }

    public class ApproveInspectionDto
    {
        public int ApprovedByUserId { get; set; }
        public string ApprovedByName { get; set; } = string.Empty;
        public string? ApprovalNote { get; set; }
    }
}
