using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;

        public UsersController(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = users.Select(MapToDto);
            return Ok(dtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();
            return Ok(MapToDto(user));
        }

        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserDto>> GetByUsername(string username)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null) return NotFound();
            return Ok(MapToDto(user));
        }

        [HttpGet("email/{email}")]
        public async Task<ActionResult<UserDto>> GetByEmail(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return NotFound();
            return Ok(MapToDto(user));
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
        {
            // Check if username exists
            var existingUser = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existingUser != null)
            {
                return BadRequest("Username already exists");
            }

            // Check if email exists
            existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                StudentId = dto.StudentId,
                StudentCode = dto.StudentCode,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = user.Id }, MapToDto(user));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UserDto dto)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.FullName = dto.FullName;
            user.Email = dto.Email;
            user.Phone = dto.Phone;
            user.Role = dto.Role;
            user.AvatarUrl = dto.AvatarUrl;
            user.IsActive = dto.IsActive;

            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpPut("{id}/password")]
        public async Task<ActionResult> UpdatePassword(int id, [FromBody] string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null) return NotFound();

            await _userRepository.DeleteAsync(user);
            return NoContent();
        }

        private static UserDto MapToDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role,
                AvatarUrl = user.AvatarUrl,
                StudentId = user.StudentId,
                StudentCode = user.StudentCode,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
