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
    [Route("api/project-reviews")]
    public class ProjectReviewController : BaseController<ProjectReview, ProjectReviewResponse, CreateProjectReviewRequest, UpdateProjectReviewRequest, ProjectReviewQueryFilter, int>
    {
        private readonly IProjectReviewService _projectReviewService;

        public ProjectReviewController(IProjectReviewService service) : base(service)
        {
            _projectReviewService = service;
        }

        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<ProjectReviewResponse>>> GetAllPagedAsync([FromQuery] ProjectReviewQueryFilter filter, CancellationToken cancellationToken = default)
            => base.GetAllPagedAsync(filter, cancellationToken);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<ProjectReviewResponse>> GetById(int id, CancellationToken cancellationToken = default)
            => base.GetById(id, cancellationToken);

        [AllowAnonymous]
        [HttpGet("project/{projectId}")]
        public async Task<ActionResult<PagedResult<ProjectReviewResponse>>> GetByProjectId(int projectId, [FromQuery] PaginationRequest pagination, CancellationToken cancellationToken = default)
        {
            var result = await _projectReviewService.GetByProjectIdAsync(projectId, pagination, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("project/{projectId}/average-rating")]
        public async Task<ActionResult<double>> GetAverageRating(int projectId, CancellationToken cancellationToken = default)
        {
            var rating = await _projectReviewService.GetAverageRatingAsync(projectId, cancellationToken);
            return Ok(rating);
        }

        [Authorize]
        [HttpGet("project/{projectId}/has-reviewed")]
        public async Task<ActionResult<bool>> HasUserReviewed(int projectId, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var hasReviewed = await _projectReviewService.HasUserReviewedAsync(userId, projectId, cancellationToken);
            return Ok(hasReviewed);
        }

        [Authorize]
        [HttpPost]
        public override async Task<ActionResult<ProjectReviewResponse>> Create([FromBody] CreateProjectReviewRequest dto, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var result = await _projectReviewService.CreateForUserAsync(userId, dto, cancellationToken);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [Authorize]
        [HttpPut("{id}")]
        public override async Task<ActionResult<ProjectReviewResponse>> Update(int id, [FromBody] UpdateProjectReviewRequest dto, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var existing = await _projectReviewService.GetByIdAsync(id, cancellationToken);

            if (existing.UserId != userId)
                return Forbid();

            return await base.Update(id, dto, cancellationToken);
        }

        [Authorize]
        [HttpDelete("{id}")]
        public override async Task<ActionResult> Delete(int id, CancellationToken cancellationToken = default)
        {
            var userId = GetCurrentUserId();
            var existing = await _projectReviewService.GetByIdAsync(id, cancellationToken);

            var isAdmin = User.IsInRole("Admin");
            if (existing.UserId != userId && !isAdmin)
                return Forbid();

            return await base.Delete(id, cancellationToken);
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
