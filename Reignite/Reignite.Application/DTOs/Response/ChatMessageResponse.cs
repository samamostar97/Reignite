namespace Reignite.Application.DTOs.Response
{
    public class ChatMessageResponse
    {
        public int Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImageUrl { get; set; }
        public int HobbyId { get; set; }
        public string HobbyName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
