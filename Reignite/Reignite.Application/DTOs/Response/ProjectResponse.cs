using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.DTOs.Response
{
    public class ProjectResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? HoursSpent { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public int HobbyId { get; set; }
        public string HobbyName { get; set; } = string.Empty;
        public int? ProductId { get; set; }
        public string? ProductName { get; set; }
        public DateTime CreatedAt { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public List<ProjectReviewResponse> Reviews { get; set; } = new();
    }
}
