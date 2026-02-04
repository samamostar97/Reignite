using Reignite.Application.DTOs.Request;
using Reignite.Application.DTOs.Response;

using Reignite.Application.IServices;
using Microsoft.AspNetCore.Mvc;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
        {
                var response = await _authService.RegisterAsync(request);
                return Ok(response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
        {
                var response = await _authService.LoginAsync(request);
                return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest request)
        {
                var response = await _authService.RefreshTokenAsync(request);
                return Ok(response);
        }
    }
}

