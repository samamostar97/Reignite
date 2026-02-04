using Reignite.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Core.Entities
{
    public class Wishlist : BaseEntity
    {
        public int UserId { get; set; }

        // Navigation properties
        public User User { get; set; } = null!;
        public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
    }
}

