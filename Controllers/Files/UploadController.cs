using Microsoft.AspNetCore.Mvc;
using NewsPage.helpers;
using NewsPage.Models.ResponseDTO;

namespace NewsPage.Controllers.Files
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly FileHelper _fileHelper;

        public UploadController(FileHelper fileHelper)
        {
            _fileHelper = fileHelper;
        }

        [HttpPost("images")]
        public async Task<IActionResult> UploadImages([FromForm] IFormFileCollection files)
        {
            try
            {
                if (files == null || files.Count == 0)
                {
                    return BadRequest(new ApiResponse<object>(400, "Vui lòng gửi ít nhất một file hình ảnh."));
                }

                //định dạng file được phép
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp", ".tiff" };
                const long maxFileSize = 50 * 1024 * 1024; // 50MB
                var uploadedFiles = new List<string>();

                foreach (var file in files)
                {
                    // Kiểm tra định dạng file
                    var fileExtension = Path.GetExtension(file.FileName).ToLower();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return BadRequest(new ApiResponse<object>(400, $"File {file.FileName} có định dạng không hợp lệ. Chỉ chấp nhận {string.Join(", ", allowedExtensions)}."));
                    }

                    // Kiểm tra  file size
                    if (file.Length > maxFileSize)
                    {
                        return BadRequest(new ApiResponse<object>(400, $"File {file.FileName} vượt quá giới hạn 50MB."));
                    }

                    // Lưu file
                    var filePath = await _fileHelper.SaveFileAsync(file);
                    uploadedFiles.Add(filePath);
                }

                return Ok(new ApiResponse<List<string>>(200, "Upload các hình ảnh thành công.", uploadedFiles));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ApiResponse<object>(400, ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<object>(500, "Có lỗi xảy ra khi upload hình ảnh.", ex.Message));
            }
        }

    }
}
