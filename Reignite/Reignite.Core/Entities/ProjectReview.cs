using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class ProjectReview : BaseEntity
    {
        public int UserId { get; set; }
        public int ProjectId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Project Project { get; set; } = null!;
    }
}
