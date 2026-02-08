using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Reignite.Application.DTOs.Response;
using Reignite.Application.IRepositories;
using Reignite.Application.IServices;
using Reignite.Core.Entities;

namespace Reignite.Infrastructure.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IRepository<Wishlist, int> _wishlistRepository;
        private readonly IMapper _mapper;

        public WishlistService(IRepository<Wishlist, int> wishlistRepository, IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _mapper = mapper;
        }

        public async Task<WishlistResponse?> GetUserWishlistAsync(int userId)
        {
            var wishlist = await _wishlistRepository
                .AsQueryable()
                .Include(w => w.WishlistItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId);

            if (wishlist == null)
                return null;

            var response = _mapper.Map<WishlistResponse>(wishlist);

            // Map items with product details
            response.Items = wishlist.WishlistItems.Select(item => new WishlistItemResponse
            {
                Id = item.Id,
                WishlistId = item.WishlistId,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                ProductPrice = item.Product.Price,
                ProductImageUrl = item.Product.ImageUrl,
                AddedAt = item.CreatedAt
            }).ToList();

            return response;
        }
    }
}
