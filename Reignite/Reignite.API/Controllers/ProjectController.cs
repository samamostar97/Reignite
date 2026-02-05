using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IServices;
using Reignite.Core.Entities;
using System.Security.Claims;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/project")]
    public class ProjectController : BaseController<Project, ProjectResponse, CreateProjectRequest, UpdateProjectRequest, ProjectQueryFilter, int>
    {
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService service) : base(service)
        {
            _projectService = service;
        }
        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<ProjectResponse>>> GetAllPagedAsync([FromQuery] ProjectQueryFilter filter) => base.GetAllPagedAsync(filter);
        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<ProjectResponse>> GetById(int id) => base.GetById(id);
        [Authorize(Roles ="Admin,AppUser")]
        [HttpPost]
        public override Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest dto) => base.Create(dto);
        [Authorize(Roles = "Admin,AppUser")]
        [HttpPut("{id}")]
        public override async Task<ActionResult<ProjectResponse>> Update(int id, [FromBody] UpdateProjectRequest dto)
        {
            if (!await IsOwnerOrAdmin(id))
                return Forbid();

            return await base.Update(id, dto);
        }
        [Authorize(Roles = "Admin,AppUser")]
        [HttpDelete("{id}")]
        public override async Task<ActionResult> Delete(int id)
        {
            if (!await IsOwnerOrAdmin(id))
                return Forbid();

            return await base.Delete(id);
        }

        [AllowAnonymous]
        [HttpGet("top-rated")]
        public async Task<ActionResult<List<ProjectResponse>>> GetTopRated([FromQuery] int count = 3)
        {
            var result = await _projectService.GetTopRatedProjectsAsync(count);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,AppUser")]
        [HttpPost("{id}/image")]
        public async Task<ActionResult<ProjectResponse>> UploadImage(int id, IFormFile file)
        {
            if (!await IsOwnerOrAdmin(id))
                return Forbid();

            if (file == null || file.Length == 0)
                return BadRequest("Nije odabrana slika");

            var fileRequest = new FileUploadRequest
            {
                FileStream = file.OpenReadStream(),
                FileName = file.FileName,
                ContentType = file.ContentType,
                FileSize = file.Length
            };

            var result = await _projectService.UploadImageAsync(id, fileRequest);
            return Ok(result);
        }

        [Authorize(Roles = "Admin,AppUser")]
        [HttpDelete("{id}/image")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            if (!await IsOwnerOrAdmin(id))
                return Forbid();

            await _projectService.DeleteImageAsync(id);
            return NoContent();
        }

        private async Task<bool> IsOwnerOrAdmin(int projectId)
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
