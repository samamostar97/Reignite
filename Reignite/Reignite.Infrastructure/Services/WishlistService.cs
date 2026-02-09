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
        private readonly IRepository<WishlistItem, int> _wishlistItemRepository;
        private readonly IRepository<Product, int> _productRepository;
        private readonly IMapper _mapper;

        public WishlistService(
            IRepository<Wishlist, int> wishlistRepository,
            IRepository<WishlistItem, int> wishlistItemRepository,
            IRepository<Product, int> productRepository,
            IMapper mapper)
        {
            _wishlistRepository = wishlistRepository;
            _wishlistItemRepository = wishlistItemRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<WishlistResponse?> GetUserWishlistAsync(int userId, CancellationToken cancellationToken = default)
        {
            var wishlist = await _wishlistRepository
                .AsQueryable()
                .Include(w => w.WishlistItems)
                    .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

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
                ProductImageUrl = item.Product.ProductImageUrl,
                AddedAt = item.CreatedAt
            }).ToList();

            return response;
        }

        public async Task<WishlistItemResponse> AddItemAsync(int userId, int productId, CancellationToken cancellationToken = default)
        {
            var product = await _productRepository.GetByIdAsync(productId, cancellationToken);
            if (product == null)
                throw new KeyNotFoundException("Proizvod nije pronađen.");

            // Get or create wishlist
            var wishlist = await _wishlistRepository
                .AsQueryable()
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

            if (wishlist == null)
            {
                wishlist = new Wishlist
                {
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                };
                await _wishlistRepository.AddAsync(wishlist, cancellationToken);
            }

            // Check if already in wishlist
            var existing = wishlist.WishlistItems.FirstOrDefault(i => i.ProductId == productId);
            if (existing != null)
                throw new InvalidOperationException("Proizvod je već na listi želja.");

            var item = new WishlistItem
            {
                WishlistId = wishlist.Id,
                ProductId = productId,
                Quantity = 1,
                UnitPrice = product.Price,
                CreatedAt = DateTime.UtcNow
            };
            await _wishlistItemRepository.AddAsync(item, cancellationToken);

            return new WishlistItemResponse
            {
                Id = item.Id,
                WishlistId = wishlist.Id,
                ProductId = product.Id,
                ProductName = product.Name,
                ProductPrice = product.Price,
                ProductImageUrl = product.ProductImageUrl,
                AddedAt = item.CreatedAt
            };
        }

        public async Task RemoveItemAsync(int userId, int productId, CancellationToken cancellationToken = default)
        {
            var wishlist = await _wishlistRepository
                .AsQueryable()
                .Include(w => w.WishlistItems)
                .FirstOrDefaultAsync(w => w.UserId == userId, cancellationToken);

            if (wishlist == null)
                throw new KeyNotFoundException("Lista želja nije pronađena.");

            var item = wishlist.WishlistItems.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
                throw new KeyNotFoundException("Proizvod nije na listi želja.");

            await _wishlistItemRepository.DeleteAsync(item, cancellationToken);
        }
    }
}
