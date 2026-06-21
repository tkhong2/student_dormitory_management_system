using BillingMaintenanceService.Application.DTOs;
using BillingMaintenanceService.Application.Interfaces;
using BillingMaintenanceService.Domain.Entities;
using BillingMaintenanceService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using OfficeOpenXml;
using System.Text;

namespace BillingMaintenanceService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly AppDbContext _context;

        public UsersController(IUserRepository userRepository, AppDbContext context)
        {
            _userRepository = userRepository;
            _context = context;
            // Set EPPlus license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDto>>> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = users.Select(MapToDto);
            return Ok(dtos);
        }

        // DEBUG: Get all users including deleted ones
        [HttpGet("debug/all-users-including-deleted")]
        public async Task<ActionResult> GetAllIncludingDeleted()
        {
            var allUsers = await _context.Users.IgnoreQueryFilters().ToListAsync();
            
            var result = allUsers.Select(u => new
            {
                u.Id,
                u.Username,
                u.FullName,
                u.Email,
                u.Role,
                u.IsActive,
                u.IsDeleted,
                u.CreatedAt,
                PasswordHashLength = u.PasswordHash != null ? u.PasswordHash.Length : 0
            }).OrderByDescending(u => u.CreatedAt).ToList();
            
            return Ok(new
            {
                TotalUsers = allUsers.Count,
                DeletedUsers = allUsers.Count(u => u.IsDeleted),
                ActiveUsers = allUsers.Count(u => !u.IsDeleted && u.IsActive),
                Users = result
            });
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
            // Log incoming data for debugging
            Console.WriteLine($"Creating user: Username={dto.Username}, HasPassword={!string.IsNullOrEmpty(dto.Password)}, PasswordLength={dto.Password?.Length}");
            
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

            // Validate password
            if (string.IsNullOrWhiteSpace(dto.Password))
            {
                return BadRequest("Password is required");
            }

            if (dto.Password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            Console.WriteLine($"Password hashed successfully. Hash length: {passwordHash.Length}");

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = passwordHash,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Role = dto.Role,
                StudentId = dto.StudentId,
                StudentCode = dto.StudentCode,
                Faculty = dto.Faculty,
                ClassCode = dto.ClassCode,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                Address = dto.Address,
                FacebookUrl = dto.FacebookUrl,
                ZaloPhone = dto.ZaloPhone,
                InstagramUrl = dto.InstagramUrl,
                LinkedInUrl = dto.LinkedInUrl,
                IsActive = true,
                IsDeleted = false,  // IMPORTANT: Set IsDeleted = false explicitly
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.AddAsync(user);
            Console.WriteLine($"User created successfully. UserId={user.Id}");
            
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
            user.StudentCode = dto.StudentCode;
            user.Faculty = dto.Faculty;
            user.ClassCode = dto.ClassCode;
            user.Gender = dto.Gender;
            user.DateOfBirth = dto.DateOfBirth;
            user.Address = dto.Address;
            user.FacebookUrl = dto.FacebookUrl;
            user.ZaloPhone = dto.ZaloPhone;
            user.InstagramUrl = dto.InstagramUrl;
            user.LinkedInUrl = dto.LinkedInUrl;
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

        /// <summary>
        /// Export template Excel for students
        /// GET: api/users/export-template
        /// </summary>
        [HttpGet("export-template")]
        public IActionResult ExportTemplate()
        {
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Mẫu Tài Khoản Sinh Viên");

            // Header row
            worksheet.Cells[1, 1].Value = "STT";
            worksheet.Cells[1, 2].Value = "Tên đăng nhập (*)";
            worksheet.Cells[1, 3].Value = "Mật khẩu (*)";
            worksheet.Cells[1, 4].Value = "Họ và tên (*)";
            worksheet.Cells[1, 5].Value = "Email (*)";
            worksheet.Cells[1, 6].Value = "Số điện thoại (*)";
            worksheet.Cells[1, 7].Value = "Mã sinh viên";
            worksheet.Cells[1, 8].Value = "Khoa";
            worksheet.Cells[1, 9].Value = "Lớp";
            worksheet.Cells[1, 10].Value = "Giới tính (Male/Female/Other)";
            worksheet.Cells[1, 11].Value = "Ngày sinh (dd/MM/yyyy)";
            worksheet.Cells[1, 12].Value = "Địa chỉ";

            // Style header
            using (var range = worksheet.Cells[1, 1, 1, 12])
            {
                range.Style.Font.Bold = true;
                range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
            }

            // Sample data rows
            worksheet.Cells[2, 1].Value = 1;
            worksheet.Cells[2, 2].Value = "N19DCCN001";
            worksheet.Cells[2, 3].Value = "123456";
            worksheet.Cells[2, 4].Value = "Nguyễn Văn A";
            worksheet.Cells[2, 5].Value = "nvana@example.com";
            worksheet.Cells[2, 6].Value = "0901234567";
            worksheet.Cells[2, 7].Value = "N19DCCN001";
            worksheet.Cells[2, 8].Value = "Công nghệ thông tin";
            worksheet.Cells[2, 9].Value = "K19-CNTT";
            worksheet.Cells[2, 10].Value = "Male";
            worksheet.Cells[2, 11].Value = "15/03/2001";
            worksheet.Cells[2, 12].Value = "Hà Nội";

            worksheet.Cells[3, 1].Value = 2;
            worksheet.Cells[3, 2].Value = "N19DCCN002";
            worksheet.Cells[3, 3].Value = "123456";
            worksheet.Cells[3, 4].Value = "Trần Thị B";
            worksheet.Cells[3, 5].Value = "tthib@example.com";
            worksheet.Cells[3, 6].Value = "0901234568";
            worksheet.Cells[3, 7].Value = "N19DCCN002";
            worksheet.Cells[3, 8].Value = "Công nghệ thông tin";
            worksheet.Cells[3, 9].Value = "K19-CNTT";
            worksheet.Cells[3, 10].Value = "Female";
            worksheet.Cells[3, 11].Value = "20/05/2001";
            worksheet.Cells[3, 12].Value = "TP.HCM";

            // Auto fit columns
            worksheet.Cells.AutoFitColumns();

            // Instructions sheet
            var instructionSheet = package.Workbook.Worksheets.Add("Hướng dẫn");
            instructionSheet.Cells[1, 1].Value = "HƯỚNG DẪN IMPORT TÀI KHOẢN SINH VIÊN";
            instructionSheet.Cells[1, 1].Style.Font.Bold = true;
            instructionSheet.Cells[1, 1].Style.Font.Size = 14;
            
            instructionSheet.Cells[3, 1].Value = "1. Các trường có dấu (*) là bắt buộc";
            instructionSheet.Cells[4, 1].Value = "2. Giới tính: Male (Nam), Female (Nữ), Other (Khác)";
            instructionSheet.Cells[5, 1].Value = "3. Ngày sinh: Định dạng dd/MM/yyyy (VD: 15/03/2001)";
            instructionSheet.Cells[6, 1].Value = "4. Tên đăng nhập không được trùng lặp";
            instructionSheet.Cells[7, 1].Value = "5. Email phải là định dạng email hợp lệ";
            instructionSheet.Cells[8, 1].Value = "6. Mật khẩu mặc định tối thiểu 6 ký tự";
            instructionSheet.Cells[9, 1].Value = "7. Vai trò tự động gán là 'Student' (Sinh viên)";
            
            instructionSheet.Cells.AutoFitColumns();

            var stream = new MemoryStream();
            package.SaveAs(stream);
            stream.Position = 0;

            var fileName = $"MauTaiKhoanSinhVien_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// Import students from Excel file
        /// POST: api/users/import-excel
        /// </summary>
        [HttpPost("import-excel")]
        public async Task<ActionResult> ImportExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            if (!file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xls"))
            {
                return BadRequest("Only Excel files (.xlsx, .xls) are allowed");
            }

            var results = new
            {
                TotalRows = 0,
                SuccessCount = 0,
                ErrorCount = 0,
                Errors = new List<string>()
            };

            try
            {
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0]; // First sheet
                var rowCount = worksheet.Dimension?.Rows ?? 0;

                if (rowCount < 2)
                {
                    return BadRequest("File không có dữ liệu");
                }

                var successCount = 0;
                var errors = new List<string>();

                // Start from row 2 (skip header)
                for (int row = 2; row <= rowCount; row++)
                {
                    try
                    {
                        var username = worksheet.Cells[row, 2].Text?.Trim();
                        var password = worksheet.Cells[row, 3].Text?.Trim();
                        var fullName = worksheet.Cells[row, 4].Text?.Trim();
                        var email = worksheet.Cells[row, 5].Text?.Trim();
                        var phone = worksheet.Cells[row, 6].Text?.Trim();
                        var studentCode = worksheet.Cells[row, 7].Text?.Trim();
                        var faculty = worksheet.Cells[row, 8].Text?.Trim();
                        var classCode = worksheet.Cells[row, 9].Text?.Trim();
                        var genderText = worksheet.Cells[row, 10].Text?.Trim();
                        var dobText = worksheet.Cells[row, 11].Text?.Trim();
                        var address = worksheet.Cells[row, 12].Text?.Trim();

                        // Validation
                        if (string.IsNullOrEmpty(username))
                        {
                            errors.Add($"Dòng {row}: Thiếu tên đăng nhập");
                            continue;
                        }

                        if (string.IsNullOrEmpty(password))
                        {
                            errors.Add($"Dòng {row}: Thiếu mật khẩu");
                            continue;
                        }

                        if (password.Length < 6)
                        {
                            errors.Add($"Dòng {row}: Mật khẩu phải có ít nhất 6 ký tự");
                            continue;
                        }

                        if (string.IsNullOrEmpty(fullName))
                        {
                            errors.Add($"Dòng {row}: Thiếu họ và tên");
                            continue;
                        }

                        if (string.IsNullOrEmpty(email))
                        {
                            errors.Add($"Dòng {row}: Thiếu email");
                            continue;
                        }

                        if (string.IsNullOrEmpty(phone))
                        {
                            errors.Add($"Dòng {row}: Thiếu số điện thoại");
                            continue;
                        }

                        // Check duplicate username
                        var existingUser = await _userRepository.GetByUsernameAsync(username);
                        if (existingUser != null)
                        {
                            errors.Add($"Dòng {row}: Tên đăng nhập '{username}' đã tồn tại");
                            continue;
                        }

                        // Check duplicate email
                        existingUser = await _userRepository.GetByEmailAsync(email);
                        if (existingUser != null)
                        {
                            errors.Add($"Dòng {row}: Email '{email}' đã tồn tại");
                            continue;
                        }

                        // Parse date of birth
                        DateOnly? dateOfBirth = null;
                        if (!string.IsNullOrEmpty(dobText))
                        {
                            if (DateTime.TryParseExact(dobText, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out DateTime parsedDate))
                            {
                                dateOfBirth = DateOnly.FromDateTime(parsedDate);
                            }
                            else
                            {
                                errors.Add($"Dòng {row}: Ngày sinh không đúng định dạng dd/MM/yyyy");
                                continue;
                            }
                        }

                        // Normalize gender
                        string? gender = null;
                        if (!string.IsNullOrEmpty(genderText))
                        {
                            gender = genderText.ToLower() switch
                            {
                                "male" or "nam" => "Male",
                                "female" or "nữ" or "nu" => "Female",
                                "other" or "khác" or "khac" => "Other",
                                _ => null
                            };
                        }

                        // Create user
                        var user = new User
                        {
                            Username = username,
                            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                            FullName = fullName,
                            Email = email,
                            Phone = phone,
                            Role = "Student", // Always Student for import
                            StudentCode = studentCode,
                            Faculty = faculty,
                            ClassCode = classCode,
                            Gender = gender,
                            DateOfBirth = dateOfBirth,
                            Address = address,
                            IsActive = true
                        };

                        await _userRepository.AddAsync(user);
                        successCount++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Dòng {row}: {ex.Message}");
                    }
                }

                return Ok(new
                {
                    TotalRows = rowCount - 1, // Exclude header
                    SuccessCount = successCount,
                    ErrorCount = errors.Count,
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing file: {ex.Message}");
            }
        }

        /// <summary>
        /// Export current users to Excel
        /// GET: api/users/export-excel
        /// </summary>
        [HttpGet("export-excel")]
        public async Task<IActionResult> ExportExcel([FromQuery] string? role = null)
        {
            try
            {
                var users = await _userRepository.GetAllAsync();
                
                // Filter by role if specified
                if (!string.IsNullOrEmpty(role))
                {
                    users = users.Where(u => u.Role == role).ToList();
                }

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Danh Sách Tài Khoản");

                // Header row
                worksheet.Cells[1, 1].Value = "STT";
                worksheet.Cells[1, 2].Value = "Tên đăng nhập";
                worksheet.Cells[1, 3].Value = "Họ và tên";
                worksheet.Cells[1, 4].Value = "Email";
                worksheet.Cells[1, 5].Value = "Số điện thoại";
                worksheet.Cells[1, 6].Value = "Vai trò";
                worksheet.Cells[1, 7].Value = "Mã sinh viên";
                worksheet.Cells[1, 8].Value = "Khoa";
                worksheet.Cells[1, 9].Value = "Lớp";
                worksheet.Cells[1, 10].Value = "Giới tính";
                worksheet.Cells[1, 11].Value = "Ngày sinh";
                worksheet.Cells[1, 12].Value = "Địa chỉ";
                worksheet.Cells[1, 13].Value = "Trạng thái";
                worksheet.Cells[1, 14].Value = "Ngày tạo";

                // Style header
                using (var range = worksheet.Cells[1, 1, 1, 14])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                    range.Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                }

                // Data rows
                int rowIndex = 2;
                foreach (var user in users)
                {
                    worksheet.Cells[rowIndex, 1].Value = rowIndex - 1;
                    worksheet.Cells[rowIndex, 2].Value = user.Username;
                    worksheet.Cells[rowIndex, 3].Value = user.FullName;
                    worksheet.Cells[rowIndex, 4].Value = user.Email;
                    worksheet.Cells[rowIndex, 5].Value = user.Phone;
                    worksheet.Cells[rowIndex, 6].Value = user.Role == "Admin" ? "Quản trị viên" : 
                                                          user.Role == "Staff" ? "Nhân viên" : "Sinh viên";
                    worksheet.Cells[rowIndex, 7].Value = user.StudentCode ?? "";
                    worksheet.Cells[rowIndex, 8].Value = user.Faculty ?? "";
                    worksheet.Cells[rowIndex, 9].Value = user.ClassCode ?? "";
                    worksheet.Cells[rowIndex, 10].Value = user.Gender == "Male" ? "Nam" : 
                                                           user.Gender == "Female" ? "Nữ" : 
                                                           user.Gender == "Other" ? "Khác" : "";
                    worksheet.Cells[rowIndex, 11].Value = user.DateOfBirth?.ToString("dd/MM/yyyy") ?? "";
                    worksheet.Cells[rowIndex, 12].Value = user.Address ?? "";
                    worksheet.Cells[rowIndex, 13].Value = user.IsActive ? "Hoạt động" : "Vô hiệu";
                    worksheet.Cells[rowIndex, 14].Value = user.CreatedAt.ToString("dd/MM/yyyy HH:mm");
                    rowIndex++;
                }

                // Auto fit columns
                worksheet.Cells.AutoFitColumns();

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                var fileName = $"DanhSachTaiKhoan_{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error exporting data: {ex.Message}");
            }
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
                Faculty = user.Faculty,
                ClassCode = user.ClassCode,
                Gender = user.Gender,
                DateOfBirth = user.DateOfBirth,
                Address = user.Address,
                FacebookUrl = user.FacebookUrl,
                ZaloPhone = user.ZaloPhone,
                InstagramUrl = user.InstagramUrl,
                LinkedInUrl = user.LinkedInUrl,
                IsActive = user.IsActive,
                LastLoginAt = user.LastLoginAt,
                CreatedAt = user.CreatedAt
            };
        }
    }
}
