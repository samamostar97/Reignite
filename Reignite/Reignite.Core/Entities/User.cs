using Reignite.Core.Enums;

namespace Reignite.Core.Entities
{
    public class User : BaseEntity
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }

        // Navigation properties
        public Wishlist? Wishlist { get; set; }
        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<UserHobby> UserHobbies { get; set; } = new List<UserHobby>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        public ICollection<ProjectReview> ProjectReviews { get; set; } = new List<ProjectReview>();
    }
}

