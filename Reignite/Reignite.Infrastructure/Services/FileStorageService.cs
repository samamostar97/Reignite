using Reignite.Application.Common;
using Reignite.Application.IServices;


namespace Reignite.Infrastructure.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _webRootPath;
        public string[] AllowedExtensions => new[] { ".jpg", ".jpeg", ".png", ".gif" };
        public long MaxFileSizeBytes => 5 * 1024 * 1024; // 5MB
        public FileStorageService(string webRootPath)
        {
            _webRootPath = webRootPath;
        }

        public async Task<FileUploadResponse> UploadAsync(FileUploadRequest request, string category, string uniqueId)
        {
            if (request.FileStream == null || request.FileSize == 0)
            {
                return new FileUploadResponse
                {
                    Success = false,
                    ErrorMessage = "Fajl nije proslijeđen"
                };
            }
            if (request.FileSize > MaxFileSizeBytes)
            {
                return new FileUploadResponse
                {
                    Success = false,
                    ErrorMessage = $"Veličina fajla prelazi maksimalnih {MaxFileSizeBytes / (1024 * 1024)}MB"
                };
            }
            var extension = Path.GetExtension(request.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                return new FileUploadResponse
                {
                    Success = false,
                    ErrorMessage = $"Dozvoljene ekstenzije su: {string.Join(", ", AllowedExtensions)}"
                };
            }

            // Validate file signature (magic bytes)
            if (!await IsValidImageFileAsync(request.FileStream))
            {
                return new FileUploadResponse
                {
                    Success = false,
                    ErrorMessage = "Fajl nije validna slika. Sadržaj fajla ne odgovara ekstenziji."
                };
            }

            var uploadsFolder = Path.Combine(_webRootPath, "uploads", category);
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{uniqueId}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await request.FileStream.CopyToAsync(fileStream);
            }

            var fileUrl = $"/uploads/{category}/{fileName}";

            return new FileUploadResponse
            {
                Success = true,
                FileUrl = fileUrl
            };
        }

        private async Task<bool> IsValidImageFileAsync(Stream fileStream)
        {
            if (!fileStream.CanSeek)
            {
                return false;
            }

            var buffer = new byte[8];
            fileStream.Position = 0;
            await fileStream.ReadAsync(buffer, 0, buffer.Length);
            fileStream.Position = 0; // Reset position for later use

            // JPEG: FF D8 FF
            if (buffer[0] == 0xFF && buffer[1] == 0xD8 && buffer[2] == 0xFF)
                return true;

            // PNG: 89 50 4E 47 0D 0A 1A 0A
            if (buffer[0] == 0x89 && buffer[1] == 0x50 && buffer[2] == 0x4E && buffer[3] == 0x47)
                return true;

            // GIF: 47 49 46 38 (GIF8)
            if (buffer[0] == 0x47 && buffer[1] == 0x49 && buffer[2] == 0x46 && buffer[3] == 0x38)
                return true;

            return false;
        }

        public Task<bool> DeleteAsync(string? fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                return Task.FromResult(false);
            }

            var relativePath = fileUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
            var filePath = Path.Combine(_webRootPath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
