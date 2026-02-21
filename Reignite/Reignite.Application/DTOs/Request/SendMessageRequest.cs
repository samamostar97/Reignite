using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class SendMessageRequest
    {
        [Required(ErrorMessage = "HobbyId je obavezan.")]
        public int HobbyId { get; set; }

        [Required(ErrorMessage = "Sadržaj poruke je obavezan.")]
        [MaxLength(500, ErrorMessage = "Poruka ne smije biti duža od 500 karaktera.")]
        public string Content { get; set; } = string.Empty;
    }
}
