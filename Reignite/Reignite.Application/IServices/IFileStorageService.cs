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
        Task<FileUploadResponse> UploadAsync(FileUploadRequest request, string category, string uniqueId, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(string? fileUrl, CancellationToken cancellationToken = default);
        string[] AllowedExtensions { get; }
        long MaxFileSizeBytes { get; }
    }
}
