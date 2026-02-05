using Reignite.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.Filters
{
    public class ProductQueryFilter : PaginationRequest
    {
        public string? Search {  get; set; }
        public string? OrderBy { get; set; }
        public int? ProductCategoryId { get; set; }
    }
}
