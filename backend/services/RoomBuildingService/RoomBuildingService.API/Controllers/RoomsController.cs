using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;
        private readonly IBuildingRepository _buildingRepository;
        private readonly IRoomTypeRepository _roomTypeRepository;
        private readonly IFloorRepository _floorRepository;
        private readonly IRoomImageRepository _roomImageRepository;

        public RoomsController(
            IRoomRepository roomRepository,
            IBuildingRepository buildingRepository,
            IRoomTypeRepository roomTypeRepository,
            IFloorRepository floorRepository,
            IRoomImageRepository roomImageRepository)
        {
            _roomRepository = roomRepository;
            _buildingRepository = buildingRepository;
            _roomTypeRepository = roomTypeRepository;
            _floorRepository = floorRepository;
            _roomImageRepository = roomImageRepository;
        }

        private static string? GetCoverImageUrl(Room room)
        {
            return room.Images?.FirstOrDefault(i => i.IsCoverImage)?.ImageUrl
                ?? room.Images?.FirstOrDefault()?.ImageUrl;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAll()
        {
            try
            {
                var rooms = await _roomRepository.GetAllAsync();
                var roomDtos = rooms.Select(r => new RoomDto
                {
                    Id = r.Id,
                    RoomNumber = r.RoomNumber,
                    FloorId = r.FloorId,
                    RoomTypeId = r.RoomTypeId,
                    BuildingId = r.Floor?.BuildingId ?? 0,
                    FloorNumber = r.Floor?.FloorNumber ?? 0,
                    BuildingName = r.Floor?.Building?.Name ?? "",
                    RoomTypeName = r.RoomType?.Name ?? "",
                    Status = r.Status,
                    CurrentOccupants = r.CurrentOccupants,
                    MaxOccupants = r.MaxOccupants,
                    Orientation = r.Orientation,
                    Notes = r.Notes,
                    IsLocked = r.IsLocked,
                    LockReason = r.LockReason,
                    QRCode = r.QRCode,
                    ImageUrl = GetCoverImageUrl(r),
                    LastInspectedAt = r.LastInspectedAt,
                    AvailableFrom = r.AvailableFrom
                }).ToList();

                return Ok(roomDtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Lỗi khi lấy danh sách phòng: {ex.Message}" });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetById(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var roomDto = new RoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                FloorId = room.FloorId,
                RoomTypeId = room.RoomTypeId,
                BuildingId = room.Floor.BuildingId,
                FloorNumber = room.Floor.FloorNumber,
                BuildingName = room.Floor.Building.Name,
                RoomTypeName = room.RoomType.Name,
                Status = room.Status,
                CurrentOccupants = room.CurrentOccupants,
                MaxOccupants = room.MaxOccupants,
                Orientation = room.Orientation,
                Notes = room.Notes,
                IsLocked = room.IsLocked,
                LockReason = room.LockReason,
                QRCode = room.QRCode,
                ImageUrl = GetCoverImageUrl(room),
                LastInspectedAt = room.LastInspectedAt,
                AvailableFrom = room.AvailableFrom
            };

            return Ok(roomDto);
        }

        [HttpGet("building/{buildingId}")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetByBuildingId(int buildingId)
        {
            var building = await _buildingRepository.GetByIdAsync(buildingId);
            if (building == null)
                return NotFound(new { message = "Không tìm thấy tòa nhà" });

            var rooms = await _roomRepository.GetByBuildingIdAsync(buildingId);
            var roomDtos = rooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                FloorId = r.FloorId,
                RoomTypeId = r.RoomTypeId,
                BuildingId = r.Floor.BuildingId,
                FloorNumber = r.Floor.FloorNumber,
                BuildingName = r.Floor.Building.Name,
                RoomTypeName = r.RoomType.Name,
                Status = r.Status,
                CurrentOccupants = r.CurrentOccupants,
                MaxOccupants = r.MaxOccupants,
                Orientation = r.Orientation,
                Notes = r.Notes,
                IsLocked = r.IsLocked,
                LockReason = r.LockReason,
                QRCode = r.QRCode,
                ImageUrl = GetCoverImageUrl(r),
                LastInspectedAt = r.LastInspectedAt,
                AvailableFrom = r.AvailableFrom
            });

            return Ok(roomDtos);
        }

        [HttpPost]
        public async Task<ActionResult<RoomDto>> Create([FromBody] CreateRoomDto dto)
        {
            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                return BadRequest(new { message = "Loại phòng không tồn tại" });

            var floor = await _floorRepository.GetByIdAsync(dto.FloorId);
            if (floor == null)
                return BadRequest(new { message = "Tầng không tồn tại" });

            var building = await _buildingRepository.GetByIdAsync(floor.BuildingId);
            if (building == null)
                return BadRequest(new { message = "Tòa nhà không tồn tại" });

            // Kiểm tra số lượng phòng hiện tại của tòa nhà
            var currentRoomCount = await _roomRepository.GetByBuildingIdAsync(building.Id);
            if (currentRoomCount.Count() >= building.TotalRooms)
            {
                return BadRequest(new 
                { 
                    message = $"Không thể thêm phòng. Tòa {building.Name} đã đủ {building.TotalRooms} phòng (hiện có {currentRoomCount.Count()} phòng)" 
                });
            }

            // Kiểm tra trùng số phòng trong cùng tầng
            var existingRoom = (await _roomRepository.GetAllAsync())
                .FirstOrDefault(r => r.FloorId == dto.FloorId && r.RoomNumber == dto.RoomNumber);
            if (existingRoom != null)
            {
                return BadRequest(new { message = $"Phòng {dto.RoomNumber} đã tồn tại ở tầng này" });
            }

            var room = new Room
            {
                RoomNumber = dto.RoomNumber,
                FloorId = dto.FloorId,
                RoomTypeId = dto.RoomTypeId,
                Status = dto.Status,
                CurrentOccupants = dto.CurrentOccupants,
                MaxOccupants = roomType.Capacity,
                Orientation = dto.Orientation,
                Notes = dto.Notes,
                IsLocked = dto.IsLocked,
                LockReason = dto.LockReason,
                QRCode = dto.QRCode,
                LastInspectedAt = dto.LastInspectedAt,
                AvailableFrom = dto.AvailableFrom
            };

            await _roomRepository.AddAsync(room);

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                var roomImage = new RoomImage
                {
                    RoomId = room.Id,
                    ImageUrl = dto.ImageUrl,
                    IsCoverImage = true,
                    SortOrder = 0
                };
                await _roomImageRepository.AddAsync(roomImage);
            }

            var roomDto = new RoomDto
            {
                Id = room.Id,
                RoomNumber = room.RoomNumber,
                FloorId = room.FloorId,
                RoomTypeId = room.RoomTypeId,
                Status = room.Status,
                CurrentOccupants = room.CurrentOccupants,
                MaxOccupants = room.MaxOccupants,
                Orientation = room.Orientation,
                Notes = room.Notes,
                IsLocked = room.IsLocked,
                LockReason = room.LockReason,
                QRCode = room.QRCode,
                ImageUrl = dto.ImageUrl,
                LastInspectedAt = room.LastInspectedAt,
                AvailableFrom = room.AvailableFrom
            };

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, roomDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateRoomDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var roomType = await _roomTypeRepository.GetByIdAsync(dto.RoomTypeId);
            if (roomType == null)
                return BadRequest(new { message = "Loại phòng không tồn tại" });

            var floor = await _floorRepository.GetByIdAsync(dto.FloorId);
            if (floor == null)
                return BadRequest(new { message = "Tầng không tồn tại" });

            room.RoomNumber = dto.RoomNumber;
            room.FloorId = dto.FloorId;
            room.RoomTypeId = dto.RoomTypeId;
            room.Status = dto.Status;
            room.CurrentOccupants = dto.CurrentOccupants;
            room.MaxOccupants = roomType.Capacity;
            room.Orientation = dto.Orientation;
            room.Notes = dto.Notes;
            room.IsLocked = dto.IsLocked;
            room.LockReason = dto.LockReason;
            room.QRCode = dto.QRCode;
            room.LastInspectedAt = dto.LastInspectedAt;
            room.AvailableFrom = dto.AvailableFrom;

            await _roomRepository.UpdateAsync(room);

            if (!string.IsNullOrWhiteSpace(dto.ImageUrl))
            {
                var existingCover = room.Images.FirstOrDefault(i => i.IsCoverImage) ?? room.Images.FirstOrDefault();
                if (existingCover != null)
                {
                    existingCover.ImageUrl = dto.ImageUrl;
                    existingCover.IsCoverImage = true;
                    await _roomImageRepository.UpdateAsync(existingCover);
                }
                else
                {
                    var roomImage = new RoomImage
                    {
                        RoomId = room.Id,
                        ImageUrl = dto.ImageUrl,
                        IsCoverImage = true,
                        SortOrder = 0
                    };
                    await _roomImageRepository.AddAsync(roomImage);
                }
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            // Kiểm tra phòng có người ở không
            if (room.CurrentOccupants > 0)
            {
                return BadRequest(new { message = "Không thể xóa phòng đang có người ở" });
            }

            await _roomRepository.DeleteAsync(room);

            return NoContent();
        }

        /// <summary>
        /// Cập nhật số người ở trong phòng và tự động cập nhật trạng thái
        /// </summary>
        [HttpPut("{id}/occupancy")]
        public async Task<ActionResult> UpdateOccupancy(int id, [FromBody] UpdateOccupancyRequest request)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            // Cập nhật số người
            var newOccupants = room.CurrentOccupants + request.Increment;
            
            // Validate
            if (newOccupants < 0)
                newOccupants = 0;
            if (newOccupants > room.MaxOccupants)
                return BadRequest(new { message = $"Phòng chỉ chứa tối đa {room.MaxOccupants} người" });

            room.CurrentOccupants = newOccupants;

            // Tự động cập nhật trạng thái dựa trên số người
            if (newOccupants == 0)
            {
                room.Status = "Available"; // Phòng trống
                room.AllowedGender = null; // Reset gender khi phòng trống
            }
            else if (newOccupants >= room.MaxOccupants)
            {
                room.Status = "Full"; // Phòng đầy
            }
            else
            {
                room.Status = "Occupied"; // Đang ở nhưng chưa đầy
            }

            await _roomRepository.UpdateAsync(room);

            return Ok(new 
            { 
                message = "Cập nhật thành công",
                currentOccupants = room.CurrentOccupants,
                maxOccupants = room.MaxOccupants,
                status = room.Status
            });
        }

        /// <summary>
        /// Cập nhật giới tính được phép ở trong phòng
        /// </summary>
        [HttpPut("{id}/gender")]
        public async Task<ActionResult> UpdateGender(int id, [FromBody] UpdateGenderRequest request)
        {
            var room = await _roomRepository.GetByIdAsync(id);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            // Validate gender value
            if (!string.IsNullOrEmpty(request.AllowedGender) && 
                request.AllowedGender != "Male" && 
                request.AllowedGender != "Female" && 
                request.AllowedGender != "Mixed")
            {
                return BadRequest(new { message = "Giới tính phải là Male, Female hoặc Mixed" });
            }

            room.AllowedGender = request.AllowedGender;
            await _roomRepository.UpdateAsync(room);

            return Ok(new 
            { 
                message = "Cập nhật giới tính phòng thành công",
                allowedGender = room.AllowedGender
            });
        }

        /// <summary>
        /// Lấy danh sách phòng còn chỗ trống (có thể lọc theo giới tính)
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetAvailableRooms([FromQuery] string? gender = null)
        {
            var rooms = await _roomRepository.GetAllAsync();
            
            // Lọc phòng còn chỗ trống
            var availableRooms = rooms.Where(r => 
                r.CurrentOccupants < r.MaxOccupants && 
                !r.IsLocked &&
                r.Status != "Maintenance" &&
                r.Status != "Closed"
            );

            // Lọc theo giới tính nếu có
            if (!string.IsNullOrEmpty(gender))
            {
                availableRooms = availableRooms.Where(r => 
                    string.IsNullOrEmpty(r.AllowedGender) || // Phòng chưa có ai
                    r.AllowedGender == "Mixed" || // Phòng mixed
                    r.AllowedGender == gender // Phòng cùng giới tính
                );
            }

            var roomDtos = availableRooms.Select(r => new RoomDto
            {
                Id = r.Id,
                RoomNumber = r.RoomNumber,
                FloorId = r.FloorId,
                RoomTypeId = r.RoomTypeId,
                BuildingId = r.Floor?.BuildingId ?? 0,
                FloorNumber = r.Floor?.FloorNumber ?? 0,
                BuildingName = r.Floor?.Building?.Name ?? "",
                RoomTypeName = r.RoomType?.Name ?? "",
                Status = r.Status,
                CurrentOccupants = r.CurrentOccupants,
                MaxOccupants = r.MaxOccupants,
                AllowedGender = r.AllowedGender,
                Orientation = r.Orientation,
                Notes = r.Notes,
                IsLocked = r.IsLocked,
                LockReason = r.LockReason,
                QRCode = r.QRCode,
                ImageUrl = GetCoverImageUrl(r),
                LastInspectedAt = r.LastInspectedAt,
                AvailableFrom = r.AvailableFrom,
                AvailableSlots = r.MaxOccupants - r.CurrentOccupants
            }).ToList();

            return Ok(roomDtos);
        }
    }

    public class UpdateOccupancyRequest
    {
        public int Increment { get; set; } // +1 để thêm, -1 để bớt
    }

    public class UpdateGenderRequest
    {
        public string? AllowedGender { get; set; } // Male / Female / Mixed / null
    }

    public class CreateRoomDto
    {
        public string RoomNumber { get; set; } = string.Empty;
        public int FloorId { get; set; }
        public int RoomTypeId { get; set; }
        public string Status { get; set; } = "Available";
        public int CurrentOccupants { get; set; }
        public int MaxOccupants { get; set; }
        public string? Orientation { get; set; }
        public string? Notes { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsLocked { get; set; } = false;
        public string? LockReason { get; set; }
        public string? QRCode { get; set; }
        public DateTime? LastInspectedAt { get; set; }
        public DateTime? AvailableFrom { get; set; }
    }
}
