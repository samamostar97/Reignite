using Reignite.Application.Common;
using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface ICommunityService
    {
        Task<PagedResult<CommunityUserResponse>> GetPublicUsersAsync(int pageNumber = 1, int pageSize = 12, int? hobbyId = null, CancellationToken cancellationToken = default);
        Task<PublicUserProfileResponse> GetPublicProfileAsync(int userId, CancellationToken cancellationToken = default);
        Task<PagedResult<ChatMessageResponse>> GetChatMessagesAsync(int hobbyId, int pageNumber = 1, int pageSize = 50, CancellationToken cancellationToken = default);
        Task<ChatMessageResponse> SaveMessageAsync(int userId, int hobbyId, string content, CancellationToken cancellationToken = default);
    }
}
