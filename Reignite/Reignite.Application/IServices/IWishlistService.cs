using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IWishlistService
    {
        Task<WishlistResponse?> GetUserWishlistAsync(int userId, CancellationToken cancellationToken = default);
        Task<WishlistItemResponse> AddItemAsync(int userId, int productId, CancellationToken cancellationToken = default);
        Task RemoveItemAsync(int userId, int productId, CancellationToken cancellationToken = default);
    }
}
