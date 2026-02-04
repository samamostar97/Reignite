using Reignite.Core.Enums;

namespace Reignite.Core.Entities
{
    // Tag users with their interests so you can match them for your "find craft buddies" feature
    public class UserHobby : BaseEntity
    {
        public int UserId { get; set; }
        public int HobbyId { get; set; }
        public SkillLevel SkillLevel { get; set; } = SkillLevel.Beginner;
        public string? Bio { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Hobby Hobby { get; set; } = null!;
    }
}
