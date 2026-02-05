using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.Common
{
    public class FileUploadResponse
    {
        public bool Success { get; set; }
        public string? FileUrl { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
