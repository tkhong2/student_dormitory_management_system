using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IJwtService _jwtService;
        public AuthController(IUserRepository userRepository, IJwtService jwtService, IWebHostEnvironment environment)
        {
            _userRepository = userRepository;
            _environment = environment;
            _jwtService = jwtService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest dto)
        {
            var user = await _userRepository.GetByUsernameAsync(dto.Username.Trim());
            if (user == null) return Unauthorized("Invalid credentials");

            if (!user.IsActive)
                return Unauthorized("Account is disabled");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid credentials");

            var tokenResult = _jwtService.GenerateToken(user);
            return Ok(new LoginResponse
            {
                Token = tokenResult.Token,
                ExpiresAt = tokenResult.ExpiresAt,
                User = ToUserResponse(user)
            });
        }

        [HttpPost("register")]
        public async Task<ActionResult<LoginResponse>> Register([FromBody] RegisterRequest dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and Password are required");

            var existing = await _userRepository.GetByUsernameAsync(dto.Username);
            if (existing != null) return Conflict("Username already exists");

            // If there are no accounts yet, allow bootstrapping an Admin in Development.
            // Otherwise, force self-register role to Student.
            var anyUsers = (await _userRepository.GetAllAsync()).Any();
            var requestedRole = ParseUserRoleOrNull(dto.Role);
            var roleToSet = UserRole.Student;
            if (!anyUsers && (_environment?.IsDevelopment() ?? false) && requestedRole.HasValue)
                roleToSet = requestedRole.Value;

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = dto.Username.Trim(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                FullName = dto.FullName ?? string.Empty,
                Email = dto.Email ?? string.Empty,
                PhoneNumber = dto.PhoneNumber,
                Role = roleToSet,
                ReferenceId = roleToSet == UserRole.Student ? dto.ReferenceId : null,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);

            var tokenResult = _jwtService.GenerateToken(user);
            return Ok(new LoginResponse
            {
                Token = tokenResult.Token,
                ExpiresAt = tokenResult.ExpiresAt,
                User = ToUserResponse(user)
            });
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<UserResponse>> GetCurrentUser()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub");
            if (!Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized();

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return NotFound();
            if (!user.IsActive) return Unauthorized("Account is disabled");

            return Ok(ToUserResponse(user));
        }

        private static UserRole? ParseUserRoleOrNull(string? input)
        {
            if (string.IsNullOrWhiteSpace(input)) return null;
            var normalized = input.Trim().ToLowerInvariant();

            if (normalized is "admin" or "administrator") return UserRole.Admin;
            if (normalized is "staff" or "nhanvien" or "nhânviên" or "nhân viên" or "employee") return UserRole.Staff;
            if (normalized is "student" or "sinhvien" or "sinhviên" or "sinh viên") return UserRole.Student;

            if (Enum.TryParse<UserRole>(input, ignoreCase: true, out var parsed))
                return parsed;

            return null;
        }

        private static UserResponse ToUserResponse(User user)
        {
            return new UserResponse
            {
                Id = user.Id,
                Username = user.Username,
                FullName = user.FullName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role.ToString(),
                ReferenceId = user.ReferenceId,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
