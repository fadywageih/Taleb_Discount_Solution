

namespace ServicesAbstraction
{
    public interface IImageService
    {
        Task<string> SaveBase64ImageAsync(string base64Image);
        string GetImageUrl(string fileName);
        bool DeleteImage(string imageUrl);
    }
}
