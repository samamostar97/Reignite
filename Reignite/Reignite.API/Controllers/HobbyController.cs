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
    [Route("api/hobbies")]
    [Authorize(Roles = "Admin")]
    public class HobbyController : BaseController<Hobby, HobbyResponse, CreateHobbyRequest, UpdateHobbyRequest, HobbyQueryFilter, int>
    {
        private readonly IHobbyService _hobbyService;

        public HobbyController(IHobbyService hobbyService) : base(hobbyService)
        {
            _hobbyService = hobbyService;
        }

        [AllowAnonymous]
        [HttpGet]
        public override Task<ActionResult<PagedResult<HobbyResponse>>> GetAllPagedAsync([FromQuery] HobbyQueryFilter filter, CancellationToken cancellationToken = default) => base.GetAllPagedAsync(filter, cancellationToken);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<HobbyResponse>> GetById(int id, CancellationToken cancellationToken = default) => base.GetById(id, cancellationToken);

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult<List<HobbyResponse>>> GetAll(CancellationToken cancellationToken = default)
        {
            var hobbies = await _hobbyService.GetAllAsync(cancellationToken);
            return Ok(hobbies);
        }
    }
}
