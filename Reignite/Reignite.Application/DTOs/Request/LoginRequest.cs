using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        public string Password { get; set; } = string.Empty;
    }
}

