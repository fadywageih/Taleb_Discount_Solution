using Microsoft.AspNetCore.Http;

namespace ServicesAbstraction
{
    public interface IAttachmentService
    {
        Task<string> UploadFileAsync(IFormFile file, string folderName);
        bool DeleteFile(string folderName, string fileName); // ← تم التغيير
    }
}
