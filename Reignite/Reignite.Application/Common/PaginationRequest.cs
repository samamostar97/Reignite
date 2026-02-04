using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.Common
{
    public class PaginationRequest
    {
        [Range(1, int.MaxValue, ErrorMessage = "PageNumber mora biti veci ili jednak 1")]
        public int PageNumber { get; set; } = 1;
        [Range(1, 100, ErrorMessage = "PageSize mora biti izmedu 1 i 100")]
        public int PageSize { get; set; } = 10;
    }
}
