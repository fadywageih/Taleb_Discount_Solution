using Microsoft.AspNetCore.Http;

namespace ServicesAbstraction
{
    public interface IExtendedImageService : IImageService
    {
        Task<string> SaveImageAsync(IFormFile image, string folderName);
        Task<bool> DeleteImageAsync(string imageUrl);
    }
}
