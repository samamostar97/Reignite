using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Reignite.Application.IServices;
using System.Security.Claims;

namespace Reignite.API.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly ICommunityService _communityService;

        public ChatHub(ICommunityService communityService)
        {
            _communityService = communityService;
        }

        public async Task JoinRoom(int hobbyId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"hobby-{hobbyId}");
        }

        public async Task LeaveRoom(int hobbyId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"hobby-{hobbyId}");
        }

        public async Task SendMessage(int hobbyId, string content)
        {
            var userIdClaim = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
                throw new HubException("Korisnik nije prijavljen.");

            if (string.IsNullOrWhiteSpace(content) || content.Length > 500)
                throw new HubException("Poruka mora imati između 1 i 500 karaktera.");

            var messageResponse = await _communityService.SaveMessageAsync(userId, hobbyId, content.Trim());

            await Clients.Group($"hobby-{hobbyId}").SendAsync("ReceiveMessage", messageResponse);
        }
    }
}
