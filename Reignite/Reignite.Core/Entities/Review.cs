using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class Review : BaseEntity
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public Product Product { get; set; } = null!;
    }
}

