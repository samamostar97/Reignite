namespace Reignite.Application.DTOs.Response
{
    public enum ActivityType
    {
        ProductReview = 0,
        ProjectReview = 1,
        NewProject = 2
    }

    public class ActivityResponse
    {
        public int Id { get; set; }
        public ActivityType Type { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int? Rating { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? UserProfileImageUrl { get; set; }
        public string? TargetName { get; set; }
        public string? TargetImageUrl { get; set; }
        public int TargetId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
