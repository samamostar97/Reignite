using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reignite.Application.Common;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IServices;

namespace Reignite.API.Controllers
{
    [ApiController]
    [Route("api/community")]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;

        public CommunityController(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        [AllowAnonymous]
        [HttpGet("users")]
        public async Task<ActionResult<PagedResult<CommunityUserResponse>>> GetUsers(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 12,
            [FromQuery] int? hobbyId = null,
            CancellationToken cancellationToken = default)
        {
            var result = await _communityService.GetPublicUsersAsync(pageNumber, pageSize, hobbyId, cancellationToken);
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("users/{id}")]
        public async Task<ActionResult<PublicUserProfileResponse>> GetUserProfile(
            int id, CancellationToken cancellationToken = default)
        {
            var result = await _communityService.GetPublicProfileAsync(id, cancellationToken);
            return Ok(result);
        }

        [Authorize]
        [HttpGet("chat/{hobbyId}/messages")]
        public async Task<ActionResult<PagedResult<ChatMessageResponse>>> GetChatMessages(
            int hobbyId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50,
            CancellationToken cancellationToken = default)
        {
            var result = await _communityService.GetChatMessagesAsync(hobbyId, pageNumber, pageSize, cancellationToken);
            return Ok(result);
        }
    }
}
