using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Response;
using Reignite.Application.Filters;
using Reignite.Application.IServices;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/activities")]
    [Authorize(Roles = "Admin")]
    public class ActivityController : ControllerBase
    {
        private readonly IActivityService _activityService;

        public ActivityController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        [HttpGet]
        public async Task<ActionResult<PagedResult<ActivityResponse>>> GetPaged([FromQuery] ActivityQueryFilter filter)
        {
            var result = await _activityService.GetPagedAsync(filter);
            return Ok(result);
        }
    }
}
