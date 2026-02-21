namespace Reignite.Application.DTOs.Response
{
    public class CommunityUserResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public string[] HobbyNames { get; set; } = Array.Empty<string>();
        public int ProjectCount { get; set; }
    }
}
