using Microsoft.AspNetCore.Mvc;
using RoomBuildingService.Application.DTOs;
using RoomBuildingService.Application.Interfaces;
using RoomBuildingService.Domain.Entities;

namespace RoomBuildingService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RoomImagesController : ControllerBase
    {
        private readonly IRoomImageRepository _roomImageRepository;
        private readonly IRoomRepository _roomRepository;

        public RoomImagesController(
            IRoomImageRepository roomImageRepository,
            IRoomRepository roomRepository)
        {
            _roomImageRepository = roomImageRepository;
            _roomRepository = roomRepository;
        }

        [HttpGet("room/{roomId}")]
        public async Task<ActionResult<IEnumerable<RoomImageDto>>> GetByRoomId(int roomId)
        {
            var room = await _roomRepository.GetByIdAsync(roomId);
            if (room == null)
                return NotFound(new { message = "Không tìm thấy phòng" });

            var images = await _roomImageRepository.GetByRoomIdAsync(roomId);
            var imageDtos = images.Select(i => new RoomImageDto
            {
                Id = i.Id,
                RoomId = i.RoomId,
                ImageUrl = i.ImageUrl,
                Caption = i.Caption,
                IsCoverImage = i.IsCoverImage,
                SortOrder = i.SortOrder
            });

            return Ok(imageDtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoomImageDto>> GetById(int id)
        {
            var image = await _roomImageRepository.GetByIdAsync(id);
            if (image == null)
                return NotFound(new { message = "Không tìm thấy ảnh" });

            var imageDto = new RoomImageDto
            {
                Id = image.Id,
                RoomId = image.RoomId,
                ImageUrl = image.ImageUrl,
                Caption = image.Caption,
                IsCoverImage = image.IsCoverImage,
                SortOrder = image.SortOrder
            };

            return Ok(imageDto);
        }

        [HttpPost]
        public async Task<ActionResult<RoomImageDto>> Create([FromBody] CreateRoomImageDto dto)
        {
            var room = await _roomRepository.GetByIdAsync(dto.RoomId);
            if (room == null)
                return BadRequest(new { message = "Phòng không tồn tại" });

            var image = new RoomImage
            {
                RoomId = dto.RoomId,
                ImageUrl = dto.ImageUrl,
                Caption = dto.Caption,
                IsCoverImage = dto.IsCoverImage,
                SortOrder = dto.SortOrder
            };

            await _roomImageRepository.AddAsync(image);

            var imageDto = new RoomImageDto
            {
                Id = image.Id,
                RoomId = image.RoomId,
                ImageUrl = image.ImageUrl,
                Caption = image.Caption,
                IsCoverImage = image.IsCoverImage,
                SortOrder = image.SortOrder
            };

            return CreatedAtAction(nameof(GetById), new { id = image.Id }, imageDto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] CreateRoomImageDto dto)
        {
            var image = await _roomImageRepository.GetByIdAsync(id);
            if (image == null)
                return NotFound(new { message = "Không tìm thấy ảnh" });

            image.ImageUrl = dto.ImageUrl;
            image.Caption = dto.Caption;
            image.IsCoverImage = dto.IsCoverImage;
            image.SortOrder = dto.SortOrder;

            await _roomImageRepository.UpdateAsync(image);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var image = await _roomImageRepository.GetByIdAsync(id);
            if (image == null)
                return NotFound(new { message = "Không tìm thấy ảnh" });

            await _roomImageRepository.DeleteAsync(image);

            return NoContent();
        }
    }
}
