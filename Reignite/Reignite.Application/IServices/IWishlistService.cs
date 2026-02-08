using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IWishlistService
    {
        Task<WishlistResponse?> GetUserWishlistAsync(int userId);
        Task<WishlistItemResponse> AddItemAsync(int userId, int productId);
        Task RemoveItemAsync(int userId, int productId);
    }
}
