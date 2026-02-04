using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}

