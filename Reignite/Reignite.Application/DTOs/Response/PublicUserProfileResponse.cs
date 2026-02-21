namespace Reignite.Application.DTOs.Response
{
    public class PublicUserProfileResponse
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string? ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<UserHobbyResponse> Hobbies { get; set; } = new();
        public List<ProjectResponse> Projects { get; set; } = new();
    }
}
