using Microsoft.AspNetCore.Mvc;

namespace ContractStudentService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FilesController> _logger;

        public FilesController(IWebHostEnvironment environment, ILogger<FilesController> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload")]
        public async Task<ActionResult<UploadResponse>> Upload([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "Không có file nào được tải lên" });

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(extension))
                return BadRequest(new { message = "Chỉ chấp nhận file ảnh (jpg, jpeg, png, gif, webp, svg)" });

            // Validate file size (5MB max)
            if (file.Length > 5 * 1024 * 1024)
                return BadRequest(new { message = "Kích thước file không được vượt quá 5MB" });

            try
            {
                // Create uploads directory if not exists
                var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return URL
                var url = $"/uploads/{fileName}";
                return Ok(new UploadResponse
                {
                    Url = url,
                    FileName = fileName,
                    OriginalFileName = file.FileName,
                    Size = file.Length
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, new { message = "Lỗi khi tải file lên" });
            }
        }

        [HttpDelete("delete")]
        public ActionResult Delete([FromQuery] string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return BadRequest(new { message = "URL không hợp lệ" });

            try
            {
                // Extract filename from URL
                var fileName = Path.GetFileName(url);
                var filePath = Path.Combine(_environment.WebRootPath, "uploads", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                    return Ok(new { message = "Đã xóa file" });
                }

                return NotFound(new { message = "Không tìm thấy file" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting file");
                return StatusCode(500, new { message = "Lỗi khi xóa file" });
            }
        }
    }

    public class UploadResponse
    {
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public long Size { get; set; }
    }
}
