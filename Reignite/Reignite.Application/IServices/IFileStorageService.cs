using Reignite.Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Reignite.Application.IServices
{
    public interface IFileStorageService
    {
        Task<FileUploadResponse> UploadAsync(FileUploadRequest request, string category, string uniqueId);
        Task<bool> DeleteAsync(string? fileUrl);
        string[] AllowedExtensions { get; }
        long MaxFileSizeBytes { get; }
    }
}
