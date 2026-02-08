using Reignite.Application.DTOs.Response;

namespace Reignite.Application.IServices
{
    public interface IWishlistService
    {
        Task<WishlistResponse?> GetUserWishlistAsync(int userId);
    }
}
