using System.ComponentModel.DataAnnotations;
using Reignite.Core.Enums;

namespace Reignite.Application.DTOs.Request
{
    public class AddUserHobbyRequest
    {
        [Required(ErrorMessage = "Hobi je obavezan.")]
        public int HobbyId { get; set; }

        public SkillLevel SkillLevel { get; set; } = SkillLevel.Beginner;

        [StringLength(500, ErrorMessage = "Bio ne smije biti duži od 500 karaktera.")]
        public string? Bio { get; set; }
    }
}
