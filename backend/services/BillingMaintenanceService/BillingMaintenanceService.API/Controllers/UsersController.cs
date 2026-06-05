using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role.ToString(),
                ReferenceId = u.ReferenceId,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });
            return Ok(dtos);
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> Me()
        {
            var uid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (uid == null || !Guid.TryParse(uid, out var userId)) return Unauthorized();
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return NotFound();
            return Ok(new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                ReferenceId = user.ReferenceId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([FromBody] CreateUserRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Username and Password are required" });

            var existing = await _userRepository.GetByUsernameAsync(dto.Username.Trim());
            if (existing != null) return Conflict(new { message = "Username already exists" });

            var u = new User
            {
                Username = dto.Username.Trim(),
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                Role = ParseUserRole(dto.Role),
                ReferenceId = dto.ReferenceId,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                IsActive = dto.IsActive,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            await _userRepository.AddAsync(u);
            return CreatedAtAction(nameof(GetAll), new { id = u.Id }, null);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Update(Guid id, [FromBody] UpdateUserRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.PhoneNumber = dto.PhoneNumber;
            user.Role = ParseUserRole(dto.Role);
            user.ReferenceId = dto.ReferenceId;
            if (dto.IsActive.HasValue)
                user.IsActive = dto.IsActive.Value;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> SetStatus(Guid id, [FromBody] SetActiveRequestDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.IsActive = dto.IsActive;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPut("{id}/password")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest(new { message = "NewPassword is required" });

            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPut("me/password")]
        public async Task<ActionResult> ChangeMyPassword([FromBody] ChangeMyPasswordRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return BadRequest(new { message = "CurrentPassword and NewPassword are required" });

            var uid = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (uid == null) return Unauthorized();

            var user = await _userRepository.GetByIdAsync(Guid.Parse(uid));
            if (user == null) return NotFound();

            if (user.IsActive == false)
                return Unauthorized(new { message = "Account is disabled" });

            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
                return Unauthorized(new { message = "Current password is incorrect" });

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            await _userRepository.DeleteAsync(user);
            return NoContent();
        }

        private static UserRole ParseUserRole(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
                throw new ArgumentException("Role is required", nameof(role));

            var normalized = role.Trim().ToLowerInvariant();
            if (normalized is "admin" or "administrator") return UserRole.Admin;
            if (normalized is "staff" or "nhanvien" or "nhânviên" or "nhân viên" or "employee") return UserRole.Staff;
            if (normalized is "student" or "sinhvien" or "sinhviên" or "sinh viên") return UserRole.Student;

            return Enum.Parse<UserRole>(role, ignoreCase: true);
        }
    }
}
