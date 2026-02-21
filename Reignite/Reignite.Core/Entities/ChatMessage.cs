namespace Reignite.Core.Entities
{
    public class ChatMessage : BaseEntity
    {
        public string Content { get; set; } = string.Empty;

        public int UserId { get; set; }
        public int HobbyId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Hobby Hobby { get; set; } = null!;
    }
}
