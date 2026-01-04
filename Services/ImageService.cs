using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Services
{
    public class ImageService : IExtendedImageService
    {
        private readonly IWebHostEnvironment _environment;
        private const string UploadsFolder = "uploads/business-images";
        public ImageService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }
        public async Task<string> SaveBase64ImageAsync(string base64Image)
        {
            if (string.IsNullOrEmpty(base64Image) || !base64Image.StartsWith("data:image"))
                return null;
            try
            {
                var match = Regex.Match(base64Image, @"^data:image/(?<type>[a-zA-Z]+);base64,(?<data>.+)$");
                if (!match.Success)
                    return null;

                var imageType = match.Groups["type"].Value;
                var base64Data = match.Groups["data"].Value;

                var extension = GetFileExtension(imageType);
                var fileName = GenerateFileName(extension);
                var filePath = GetFilePath(fileName);

                await EnsureDirectoryExists();
                await SaveImageFile(base64Data, filePath);

                return GetImageUrl(fileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public string GetImageUrl(string fileName)
        {
            return $"/{UploadsFolder}/{fileName}";
        }

        public bool DeleteImage(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith($"/{UploadsFolder}/"))
                    return false;

                var fileName = Path.GetFileName(imageUrl);
                var filePath = GetFilePath(fileName);

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> SaveImageAsync(IFormFile image, string folderName)
        {
            if (image == null || image.Length == 0)
                return null;

            try
            {
                var extension = Path.GetExtension(image.FileName);
                if (string.IsNullOrEmpty(extension) || !IsImageExtension(extension))
                    return null;

                var fileName = GenerateFileName(extension.Substring(1)); // Remove the dot
                var uploadsPath = Path.Combine(_environment.WebRootPath, folderName);
                var filePath = Path.Combine(uploadsPath, fileName);
                if (!Directory.Exists(uploadsPath))
                    Directory.CreateDirectory(uploadsPath);
                using var stream = new FileStream(filePath, FileMode.Create);
                await image.CopyToAsync(stream);
                return $"/{folderName}/{fileName}";
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteImageAsync(string imageUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(imageUrl))
                    return false;
                var fileName = Path.GetFileName(imageUrl);
                var filePath = Path.Combine(_environment.WebRootPath, UploadsFolder, fileName);
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool IsImageExtension(string extension)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            return allowedExtensions.Contains(extension.ToLower());
        }

        private string GetFileExtension(string imageType)
        {
            return imageType.ToLower() switch
            {
                "jpeg" or "jpg" => "jpg",
                "png" => "png",
                "gif" => "gif",
                "webp" => "webp",
                _ => "jpg"
            };
        }
        private string GenerateFileName(string extension)
        {
            return $"business_{Guid.NewGuid()}.{extension}";
        }
        private string GetFilePath(string fileName)
        {
            return Path.Combine(_environment.WebRootPath, UploadsFolder, fileName);
        }
        private async Task EnsureDirectoryExists()
        {
            var uploadsPath = Path.Combine(_environment.WebRootPath, UploadsFolder);
            if (!Directory.Exists(uploadsPath))
                Directory.CreateDirectory(uploadsPath);
        }
        private async Task SaveImageFile(string base64Data, string filePath)
        {
            var imageBytes = Convert.FromBase64String(base64Data);
            await File.WriteAllBytesAsync(filePath, imageBytes);
        }
    }
}