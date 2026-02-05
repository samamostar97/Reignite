namespace Reignite.Core.Entities
{
    // Users showcase what they've built using the kits.
    // Think of it as a gallery where someone who bought a
    // "Leather Crafting Starter Kit" can upload their finished
    // wallet with photos and descriptions.
    public class Project : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? HoursSpent { get; set; }

        public int UserId { get; set; }
        public int HobbyId { get; set; }
        public int? ProductId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Hobby Hobby { get; set; } = null!;
        public Product? Product { get; set; }
        public ICollection<ProjectReview> Reviews { get; set; } = new List<ProjectReview>();
    }
}
