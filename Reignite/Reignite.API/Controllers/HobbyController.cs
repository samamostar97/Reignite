using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IServices;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/hobby")]
    public class HobbyController : ControllerBase
    {
        private readonly IHobbyService _hobbyService;

        public HobbyController(IHobbyService hobbyService)
        {
            _hobbyService = hobbyService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<HobbyResponse>>> GetAll()
        {
            var hobbies = await _hobbyService.GetAllAsync();
            return Ok(hobbies);
        }
    }
}
