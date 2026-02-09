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
        private readonly IUserService _userService;

        public UploadsController(IProductService productService, IProjectService projectService, IUserService userService)
        {
            _productService = productService;
            _projectService = projectService;
            _userService = userService;
        }

        [HttpPost("products/{productId:int}")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(MaxImageBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxImageBytes)]
        public async Task<IActionResult> UploadProductImage(int productId, IFormFile file, CancellationToken cancellationToken = default)
        {
            var validationResult = ValidateFile(file);
            if (validationResult != null)
                return validationResult;

            var fileRequest = CreateFileRequest(file);
            var result = await _productService.UploadImageAsync(productId, fileRequest, cancellationToken);
            return Ok(new { fileUrl = result.ProductImageUrl });
        }

        [HttpDelete("products/{productId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProductImage(int productId, CancellationToken cancellationToken = default)
        {
            await _productService.DeleteImageAsync(productId, cancellationToken);
            return NoContent();
        }

        [HttpPost("projects/{projectId:int}")]
        [Authorize(Roles = "Admin,AppUser")]
        [RequestSizeLimit(MaxImageBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxImageBytes)]
        public async Task<IActionResult> UploadProjectImage(int projectId, IFormFile file, CancellationToken cancellationToken = default)
        {
            if (!await IsProjectOwnerOrAdmin(projectId, cancellationToken))
                return Forbid();

            var validationResult = ValidateFile(file);
            if (validationResult != null)
                return validationResult;

            var fileRequest = CreateFileRequest(file);
            var result = await _projectService.UploadImageAsync(projectId, fileRequest, cancellationToken);
            return Ok(new { fileUrl = result.ImageUrl });
        }

        [HttpDelete("projects/{projectId:int}")]
        [Authorize(Roles = "Admin,AppUser")]
        public async Task<IActionResult> DeleteProjectImage(int projectId, CancellationToken cancellationToken = default)
        {
            if (!await IsProjectOwnerOrAdmin(projectId, cancellationToken))
                return Forbid();

            await _projectService.DeleteImageAsync(projectId, cancellationToken);
            return NoContent();
        }

        [HttpPost("users/{userId:int}")]
        [Authorize(Roles = "Admin")]
        [RequestSizeLimit(MaxImageBytes)]
        [RequestFormLimits(MultipartBodyLengthLimit = MaxImageBytes)]
        public async Task<IActionResult> UploadUserImage(int userId, IFormFile file, CancellationToken cancellationToken = default)
        {
            var validationResult = ValidateFile(file);
            if (validationResult != null)
                return validationResult;

            var fileRequest = CreateFileRequest(file);
            var result = await _userService.UploadImageAsync(userId, fileRequest, cancellationToken);
            return Ok(new { fileUrl = result.ProfileImageUrl });
        }

        [HttpDelete("users/{userId:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserImage(int userId, CancellationToken cancellationToken = default)
        {
            await _userService.DeleteImageAsync(userId, cancellationToken);
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

        private async Task<bool> IsProjectOwnerOrAdmin(int projectId, CancellationToken cancellationToken = default)
        {
            if (User.IsInRole("Admin"))
                return true;

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId))
                return false;

            var project = await _projectService.GetByIdAsync(projectId, cancellationToken);
            return project != null && project.UserId.ToString() == currentUserId;
        }
    }
}
