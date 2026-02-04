namespace Reignite.Application.DTOs.Response
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public UserAuthResponse User { get; set; } = null!;
    }
}

