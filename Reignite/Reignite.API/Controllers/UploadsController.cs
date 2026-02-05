using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.IServices;
using System.Security.Claims;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/uploads")]
    [Authorize]
    public class UploadsController : ControllerBase
    {
        private const long MaxImageBytes = 5 * 1024 * 1024;
        private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/gif" };

        private readonly IProductService _productService;
        private readonly IProjectService _projectService;

        public UploadsController(IProductService productService, IProjectService projectService)
        {
            _productService = productService;
            _projectService = projectService;
        }

        [HttpPost("products/{productId:int}")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(MaxImageBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxImageBytes)]
        public async Task<IActionResult> UploadProductImage(int productId, IFormFile file)
        {
            var validationResult = ValidateFile(file);
            if (validationResult != null)
                return validationResult;

            var fileRequest = CreateFileRequest(file);
            var result = await _productService.UploadImageAsync(productId, fileRequest);
            return Ok(new { fileUrl = result.ProductImageUrl });
        }

        [HttpDelete("products/{productId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductImage(int productId)
        {
            await _productService.DeleteImageAsync(productId);
            return NoContent();
        }

        [HttpPost("projects/{projectId:int}")]
        [Authorize(Roles = "Admin,AppUser")]
        [RequestSizeLimit(MaxImageBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxImageBytes)]
        public async Task<IActionResult> UploadProjectImage(int projectId, IFormFile file)
        {
            if (!await IsProjectOwnerOrAdmin(projectId))
                return Forbid();

            var validationResult = ValidateFile(file);
            if (validationResult != null)
                return validationResult;

            var fileRequest = CreateFileRequest(file);
            var result = await _projectService.UploadImageAsync(projectId, fileRequest);
            return Ok(new { fileUrl = result.ImageUrl });
        }

        [HttpDelete("projects/{projectId:int}")]
        [Authorize(Roles = "Admin,AppUser")]
        public async Task<IActionResult> DeleteProjectImage(int projectId)
        {
            if (!await IsProjectOwnerOrAdmin(projectId))
                return Forbid();

            await _projectService.DeleteImageAsync(projectId);
            return NoContent();
        }

        private ActionResult? ValidateFile(IFormFile? file)
        {
            if (file == null || file.Length <= 0)
                return BadRequest("Nije odabrana slika.");

            if (file.Length > MaxImageBytes)
                return BadRequest("Slika je prevelika (max 5MB).");

            if (!AllowedContentTypes.Contains(file.ContentType))
                return BadRequest("Neispravan tip fajla (dozvoljeno: jpg, png, gif).");

            return null;
        }

        private static FileUploadRequest CreateFileRequest(IFormFile file)
        {
            return new FileUploadRequest
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length
            };
        }

        private async Task<bool> IsProjectOwnerOrAdmin(int projectId)
        {
            if (User.IsInRole("Admin"))
                return true;

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return false;

            var project = await _projectService.GetByIdAsync(projectId);
            return project != null && project.UserId.ToString() == currentUserId;
        }
    }
}
