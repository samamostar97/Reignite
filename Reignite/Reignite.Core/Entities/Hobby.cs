namespace Reignite.Core.Entities
{
    public class Hobby : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? IconUrl { get; set; }

        // Navigation properties
        public ICollection<UserHobby> UserHobbies { get; set; } = new List<UserHobby>();
        public ICollection<Project> Projects { get; set; } = new List<Project>();
    }
}
