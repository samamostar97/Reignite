using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

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
        public override Task<ActionResult<ProjectResponse>> Update(int id, [FromBody] UpdateProjectRequest dto) => base.Update(id, dto);
        [Authorize(Roles = "Admin,AppUser")]
        [HttpDelete("{id}")]
        public override Task<ActionResult> Delete(int id) => base.Delete(id);

        [AllowAnonymous]
        [HttpGet("top-rated")]
        public async Task<ActionResult<List<ProjectResponse>>> GetTopRated([FromQuery] int count = 3)
        {
            var result = await _projectService.GetTopRatedProjectsAsync(count);
            return Ok(result);
        }
    }
}
