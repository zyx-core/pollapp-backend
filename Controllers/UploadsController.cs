using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PollApp.Backend.Services;
using System.Threading.Tasks;

namespace PollApp.Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UploadsController : ControllerBase
    {
        private readonly IFileStorageService _fileStorageService;

        public UploadsController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "No file uploaded." });
            }

            // You can add logic to validate file types here if needed
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            if (System.Array.IndexOf(allowedExtensions, extension) < 0)
            {
                return BadRequest(new { error = "Invalid file type. Only images are allowed." });
            }

            try
            {
                var fileUrl = await _fileStorageService.SaveFileAsync(file, "polls");
                return Ok(new { url = fileUrl });
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, new { error = "Internal server error: " + ex.Message });
            }
        }
    }
}
