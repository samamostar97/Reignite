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
    [Route("api/projects")]
    public class ProjectController : BaseController<Project, ProjectResponse, CreateProjectRequest, UpdateProjectRequest, ProjectQueryFilter, int>
    {
        private readonly IProjectService _projectService;

        public ProjectController(IProjectService service) : base(service)
        {
            _projectService = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<ProjectResponse>>> GetAllPagedAsync([FromQuery] ProjectQueryFilter filter, CancellationToken cancellationToken = default)
            => base.GetAllPagedAsync(filter, cancellationToken);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<ProjectResponse>> GetById(int id, CancellationToken cancellationToken = default)
            => base.GetById(id, cancellationToken);

        [Authorize(Roles = "Admin,AppUser")]
        [HttpPost]
        public override async Task<ActionResult<ProjectResponse>> Create([FromBody] CreateProjectRequest dto, CancellationToken cancellationToken = default)
        {
            // For regular users, force UserId from authenticated user (security)
            // Admins can create projects on behalf of users using the UserId from request
            if (!User.IsInRole("Admin"))
            {
                var currentUserId = GetCurrentUserId();
                dto.UserId = currentUserId;
            }

            return await base.Create(dto, cancellationToken);
        }
        [Authorize(Roles = "Admin,AppUser")]
        [HttpPut("{id}")]
        public override async Task<ActionResult<ProjectResponse>> Update(int id, [FromBody] UpdateProjectRequest dto, CancellationToken cancellationToken = default)
        {
            if (!await IsOwnerOrAdmin(id, cancellationToken))
                return Forbid();

            return await base.Update(id, dto, cancellationToken);
        }
        [Authorize(Roles = "Admin,AppUser")]
        [HttpDelete("{id}")]
        public override async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            if (!await IsOwnerOrAdmin(id, cancellationToken))
                return Forbid();

            return await base.Delete(id, cancellationToken);
        }

        [AllowAnonymous]
        [HttpGet("top-rated")]
        public async Task<ActionResult<PagedResult<ProjectResponse>>> GetTopRated([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 3, CancellationToken cancellationToken = default)
        {
            var result = await _projectService.GetTopRatedProjectsAsync(pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }

        private async Task<bool> IsOwnerOrAdmin(int projectId, CancellationToken cancellationToken = default)
        {
            if (User.IsInRole("Admin"))
                return true;

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(currentUserId) || !int.TryParse(currentUserId, out var userId))
                return false;

            // Efficient ownership check without loading the full project
            return await _projectService.IsOwnerAsync(projectId, userId, cancellationToken);
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
