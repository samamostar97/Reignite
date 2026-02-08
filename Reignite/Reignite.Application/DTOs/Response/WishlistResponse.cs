namespace Reignite.Application.DTOs.Response
{
    public class WishlistResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<WishlistItemResponse> Items { get; set; } = new();
    }

    public class WishlistItemResponse
    {
        public int Id { get; set; }
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal ProductPrice { get; set; }
        public string? ProductImageUrl { get; set; }
        public DateTime AddedAt { get; set; }
    }
}
