using System.ComponentModel.DataAnnotations;

namespace Reignite.Core.Entities
{
    public class RefreshToken : BaseEntity
    {
        [Required]
        [MaxLength(256)]
        public string Token { get; set; } = string.Empty;

        public int UserId { get; set; }

        public DateTime ExpiresAt { get; set; }

        public DateTime? RevokedAt { get; set; }

        public bool IsRevoked => RevokedAt != null;

        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;

        public bool IsActive => !IsRevoked && !IsExpired;

        // Navigation property
        public User User { get; set; } = null!;
    }
}

