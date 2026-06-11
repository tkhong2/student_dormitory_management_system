using Microsoft.AspNetCore.Mvc;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Auth;

namespace BillingMaintenanceService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly JwtService _jwtService;

    public AuthController(IUserRepository userRepository, JwtService jwtService)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            
            if (user == null)
            {
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng" });
            }

            // Check if account is locked
            if (user.LockedUntil.HasValue && user.LockedUntil > DateTime.UtcNow)
            {
                return Unauthorized(new { message = $"Tài khoản đã bị khóa đến {user.LockedUntil.Value.ToLocalTime():dd/MM/yyyy HH:mm}" });
            }

            // Check if account is active
            if (!user.IsActive)
            {
                return Unauthorized(new { message = "Tài khoản đã bị vô hiệu hóa" });
            }

            // Verify password
            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                // Increment failed attempts
                user.FailedLoginAttempts++;
                
                // Lock account after 5 failed attempts
                if (user.FailedLoginAttempts >= 5)
                {
                    user.LockedUntil = DateTime.UtcNow.AddMinutes(15);
                }
                
                await _userRepository.UpdateAsync(user);
                
                return Unauthorized(new { message = "Tên đăng nhập hoặc mật khẩu không đúng" });
            }

            // Reset failed attempts on successful login
            user.FailedLoginAttempts = 0;
            user.LockedUntil = null;
            user.LastLoginAt = DateTime.UtcNow;
            user.LastLoginIp = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Generate JWT token
            var token = _jwtService.GenerateToken(user.Id, user.Username, user.Role);
            
            // Generate refresh token
            var refreshToken = _jwtService.GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);

            await _userRepository.UpdateAsync(user);

            return Ok(new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.Phone,
                    Role = user.Role,
                    AvatarUrl = user.AvatarUrl,
                    StudentCode = user.StudentCode,
                    Gender = user.Gender,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi đăng nhập", error = ex.Message });
        }
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            // Check if username already exists
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                return BadRequest(new { message = "Tên đăng nhập đã tồn tại" });
            }

            // Check if email already exists
            var existingEmail = await _userRepository.GetByEmailAsync(request.Email);
            if (existingEmail != null)
            {
                return BadRequest(new { message = "Email đã được sử dụng" });
            }

            // Create new user
            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone ?? "",
                Role = request.Role ?? "Student", // Default to Student
                StudentCode = request.StudentCode,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);

            // If role is Student, create Student record in ContractStudentService
            if (user.Role == "Student" && !string.IsNullOrWhiteSpace(request.StudentCode))
            {
                try
                {
                    using var httpClient = new HttpClient();
                    httpClient.BaseAddress = new Uri("http://localhost:5059"); // ContractStudentService port
                    
                    var studentDto = new
                    {
                        studentCode = request.StudentCode,
                        fullName = request.FullName,
                        gender = request.Gender ?? "Male",
                        dateOfBirth = request.DateOfBirth?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddYears(-20).ToString("yyyy-MM-dd"),
                        phone = request.Phone ?? "",
                        email = request.Email,
                        identityCard = request.IdentificationCard ?? "",
                        identityCardIssuedDate = DateTime.Now.AddYears(-5).ToString("yyyy-MM-dd"),
                        identityCardIssuedPlace = "Chưa cập nhật",
                        permanentAddress = "Chưa cập nhật",
                        emergencyContactName = "Chưa cập nhật",
                        emergencyContactPhone = "0000000000",
                        emergencyContactRelation = "Gia đình",
                        faculty = "Chưa cập nhật",
                        major = "Chưa cập nhật",
                        academicYear = DateTime.Now.Year,
                        isActive = true,
                        userId = user.Id
                    };

                    var response = await httpClient.PostAsJsonAsync("/api/students", studentDto);
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        // Log error but don't fail registration
                        Console.WriteLine($"Failed to create student record: {await response.Content.ReadAsStringAsync()}");
                    }
                }
                catch (Exception ex)
                {
                    // Log error but don't fail registration
                    Console.WriteLine($"Error creating student record: {ex.Message}");
                }
            }

            return Ok(new { message = "Đăng ký thành công", userId = user.Id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi đăng ký", error = ex.Message });
        }
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        try
        {
            var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
            
            if (user == null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                return Unauthorized(new { message = "Refresh token không hợp lệ hoặc đã hết hạn" });
            }

            // Generate new tokens
            var newToken = _jwtService.GenerateToken(user.Id, user.Username, user.Role);
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepository.UpdateAsync(user);

            return Ok(new
            {
                token = newToken,
                refreshToken = newRefreshToken
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi làm mới token", error = ex.Message });
        }
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        try
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return NotFound(new { message = "Người dùng không tồn tại" });
            }

            // Verify old password
            if (!BCrypt.Net.BCrypt.Verify(request.OldPassword, user.PasswordHash))
            {
                return BadRequest(new { message = "Mật khẩu cũ không đúng" });
            }

            // Update password
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            await _userRepository.UpdateAsync(user);

            return Ok(new { message = "Đổi mật khẩu thành công" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Lỗi đổi mật khẩu", error = ex.Message });
        }
    }
}

// DTOs
public class LoginRequest
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class LoginResponse
{
    public string Token { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public UserDto User { get; set; } = null!;
}

public class RegisterRequest
{
    // Account info
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string FullName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? Phone { get; set; }
    public string? Role { get; set; }
    
    // Student info
    public string? StudentCode { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? Gender { get; set; }
    public string? IdentificationCard { get; set; }
}

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = null!;
}

public class ChangePasswordRequest
{
    public int UserId { get; set; }
    public string OldPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}
