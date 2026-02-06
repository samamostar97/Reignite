namespace Reignite.Application.Options
{
    public class JwtSettings
    {
        public string Secret { get; set; } = string.Empty;
        public string Issuer { get; set; } = "Reignite";
        public string Audience { get; set; } = "ReigniteApp";
        public int AccessTokenExpirationHours { get; set; } = 24;
        public int RefreshTokenExpirationDays { get; set; } = 7;
    }
}
