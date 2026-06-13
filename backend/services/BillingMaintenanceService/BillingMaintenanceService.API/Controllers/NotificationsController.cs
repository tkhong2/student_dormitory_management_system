using Microsoft.AspNetCore.Mvc;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserRepository _userRepository;

        public NotificationsController(
            INotificationRepository notificationRepository,
            IUserRepository userRepository)
        {
            _notificationRepository = notificationRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Lấy tất cả thông báo
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAll()
        {
            var notifications = await _notificationRepository.GetAllAsync();
            return Ok(notifications.Select(n => new
            {
                n.Id,
                n.UserId,
                UserName = n.User?.FullName, // Use null-conditional operator
                n.Title,
                n.Body,
                n.Type,
                n.ActionUrl,
                n.IconType,
                n.IsRead,
                n.ReadAt,
                n.RelatedEntityId,
                n.RelatedEntityType,
                n.CreatedAt
            }));
        }

        /// <summary>
        /// Lấy thông báo theo user ID
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetByUserId(int userId)
        {
            // Note: Skip user validation since studentId comes from different service (ContractStudentService)
            // var user = await _userRepository.GetByIdAsync(userId);
            // if (user == null)
            //     return NotFound(new { message = "Không tìm thấy người dùng" });

            var notifications = await _notificationRepository.GetByUserIdAsync(userId);
            return Ok(notifications.Select(n => new
            {
                n.Id,
                n.UserId,
                n.Title,
                n.Body,
                n.Type,
                n.ActionUrl,
                n.IconType,
                n.IsRead,
                n.ReadAt,
                n.RelatedEntityId,
                n.RelatedEntityType,
                n.CreatedAt
            }));
        }

        /// <summary>
        /// Lấy thông báo chưa đọc theo user ID
        /// </summary>
        [HttpGet("user/{userId}/unread")]
        public async Task<ActionResult<IEnumerable<object>>> GetUnreadByUserId(int userId)
        {
            // Note: Skip user validation since studentId comes from different service (ContractStudentService)
            // var user = await _userRepository.GetByIdAsync(userId);
            // if (user == null)
            //     return NotFound(new { message = "Không tìm thấy người dùng" });

            var notifications = await _notificationRepository.GetUnreadByUserIdAsync(userId);
            return Ok(notifications.Select(n => new
            {
                n.Id,
                n.UserId,
                n.Title,
                n.Body,
                n.Type,
                n.ActionUrl,
                n.IconType,
                n.IsRead,
                n.ReadAt,
                n.RelatedEntityId,
                n.RelatedEntityType,
                n.CreatedAt
            }));
        }

        /// <summary>
        /// Đếm số thông báo chưa đọc
        /// </summary>
        [HttpGet("user/{userId}/unread/count")]
        public async Task<ActionResult<object>> GetUnreadCount(int userId)
        {
            // Note: Skip user validation since studentId comes from different service (ContractStudentService)
            // var user = await _userRepository.GetByIdAsync(userId);
            // if (user == null)
            //     return NotFound(new { message = "Không tìm thấy người dùng" });

            var count = await _notificationRepository.GetUnreadCountByUserIdAsync(userId);
            return Ok(new { count });
        }

        /// <summary>
        /// Lấy thông báo theo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetById(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            return Ok(new
            {
                notification.Id,
                notification.UserId,
                UserName = notification.User?.FullName,
                notification.Title,
                notification.Body,
                notification.Type,
                notification.ActionUrl,
                notification.IconType,
                notification.IsRead,
                notification.ReadAt,
                notification.RelatedEntityId,
                notification.RelatedEntityType,
                notification.CreatedAt
            });
        }

        /// <summary>
        /// Tạo thông báo mới (dành cho admin/nhân viên gửi thông báo cho sinh viên)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<object>> Create([FromBody] CreateNotificationDto dto)
        {
            // Note: Skip user validation for now since studentId comes from different service
            // Validate user exists - COMMENTED OUT for students from ContractStudentService
            // var user = await _userRepository.GetByIdAsync(dto.UserId);
            // if (user == null)
            //     return BadRequest(new { message = "Người dùng không tồn tại" });

            var notification = new Notification
            {
                UserId = dto.UserId,
                Title = dto.Title,
                Body = dto.Body,
                Type = dto.Type,
                ActionUrl = dto.ActionUrl,
                IconType = dto.IconType ?? "info",
                RelatedEntityId = dto.RelatedEntityId,
                RelatedEntityType = dto.RelatedEntityType,
                IsRead = false
            };

            await _notificationRepository.AddAsync(notification);

            return CreatedAtAction(nameof(GetById), new { id = notification.Id }, new
            {
                notification.Id,
                notification.UserId,
                notification.Title,
                notification.Body,
                notification.Type,
                notification.ActionUrl,
                notification.IconType,
                notification.IsRead,
                notification.CreatedAt
            });
        }

        /// <summary>
        /// Gửi thông báo cho nhiều người dùng (broadcast)
        /// </summary>
        [HttpPost("broadcast")]
        public async Task<ActionResult<object>> Broadcast([FromBody] BroadcastNotificationDto dto)
        {
            var createdNotifications = new List<Notification>();

            foreach (var userId in dto.UserIds)
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null) continue; // Skip invalid users

                var notification = new Notification
                {
                    UserId = userId,
                    Title = dto.Title,
                    Body = dto.Body,
                    Type = dto.Type,
                    ActionUrl = dto.ActionUrl,
                    IconType = dto.IconType ?? "info",
                    IsRead = false
                };

                await _notificationRepository.AddAsync(notification);
                createdNotifications.Add(notification);
            }

            return Ok(new
            {
                message = $"Đã gửi {createdNotifications.Count} thông báo",
                count = createdNotifications.Count
            });
        }

        /// <summary>
        /// Đánh dấu đã đọc
        /// </summary>
        [HttpPut("{id}/read")]
        public async Task<ActionResult> MarkAsRead(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            await _notificationRepository.MarkAsReadAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Đánh dấu tất cả đã đọc
        /// </summary>
        [HttpPut("user/{userId}/read-all")]
        public async Task<ActionResult> MarkAllAsRead(int userId)
        {
            // Note: Skip user validation since studentId comes from different service (ContractStudentService)
            // var user = await _userRepository.GetByIdAsync(userId);
            // if (user == null)
            //     return NotFound(new { message = "Không tìm thấy người dùng" });

            await _notificationRepository.MarkAllAsReadAsync(userId);
            return NoContent();
        }

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var notification = await _notificationRepository.GetByIdAsync(id);
            if (notification == null)
                return NotFound(new { message = "Không tìm thấy thông báo" });

            await _notificationRepository.DeleteAsync(notification);
            return NoContent();
        }
    }

    // DTOs
    public class CreateNotificationDto
    {
        public int UserId { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string Type { get; set; } = "System"; // System / ApplicationApproved / InvoiceCreated / MaintenanceDone / ContractExpiring
        public string? ActionUrl { get; set; }
        public string? IconType { get; set; } // success / warning / info / error
        public int? RelatedEntityId { get; set; }
        public string? RelatedEntityType { get; set; }
    }

    public class BroadcastNotificationDto
    {
        public List<int> UserIds { get; set; } = new();
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
        public string Type { get; set; } = "System";
        public string? ActionUrl { get; set; }
        public string? IconType { get; set; }
    }
}
