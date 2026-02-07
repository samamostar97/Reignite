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
    [Route("api/hobby")]
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
        public override Task<ActionResult<PagedResult<HobbyResponse>>> GetAllPagedAsync([FromQuery] HobbyQueryFilter filter) => base.GetAllPagedAsync(filter);

        [AllowAnonymous]
        [HttpGet("{id}")]
        public override Task<ActionResult<HobbyResponse>> GetById(int id) => base.GetById(id);

        [AllowAnonymous]
        [HttpGet("all")]
        public async Task<ActionResult<List<HobbyResponse>>> GetAll()
        {
            var hobbies = await _hobbyService.GetAllAsync();
            return Ok(hobbies);
        }
    }
}
