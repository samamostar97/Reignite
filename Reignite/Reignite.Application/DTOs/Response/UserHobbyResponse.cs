using Reignite.Core.Enums;

namespace Reignite.Application.DTOs.Response
{
    public class UserHobbyResponse
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HobbyId { get; set; }
        public string HobbyName { get; set; } = string.Empty;
        public SkillLevel SkillLevel { get; set; }
        public string? Bio { get; set; }
    }
}
