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
    [Authorize(Roles = "Admin")]
    public class ProjectController : BaseController<Project, ProjectResponse, CreateProjectRequest, UpdateProjectRequest, ProjectQueryFilter, int>
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService service) : base(service)
        {
            _projectService = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<ProjectResponse>>> GetAllPagedAsync([FromQuery] ProjectQueryFilter filter)
            => base.GetAllPagedAsync(filter);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<ProjectResponse>> GetById(int id)
            => base.GetById(id);

        [Authorize(Roles = "Admin,AppUser")]
        [HttpPost]
        public override async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest dto)
        {
            // For regular users, force UserId from authenticated user (security)
            // Admins can create projects on behalf of users using the UserId from request
            if (!User.IsInRole("Admin"))
            {
                var currentUserId = GetCurrentUserId();
                dto.UserId = currentUserId;
            }

            return await base.Create(dto);
        }
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
        public async Task<ActionResult<PagedResult<ProjectResponse>>> GetTopRated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3)
        {
            var result = await _projectService.GetTopRatedProjectsAsync(pageNumber, pageSize);
            return Ok(result);
        }

        private async Task<bool> IsOwnerOrAdmin(int projectId)
        {
            if (User.IsInRole("Admin"))
                return true;

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var userId))
                return false;

            // Efficient ownership check without loading the full project
            return await _projectService.IsOwnerAsync(projectId, userId);
        }

        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new UnauthorizedAccessException("Korisnik nije prijavljen.");

            return userId;
        }
    }
}
