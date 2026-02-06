using Reignite.Application.Common;
using Reignite.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.Filters
{
    public class OrderQueryFilter : PaginationRequest
    {
        public string? Search { get; set; }
        public string? OrderBy { get; set; }
        public int? UserId { get; set; }
        public OrderStatus? Status { get; set; }
    }
}
