using Microsoft.AspNetCore.Http;

namespace FileStorage.Application.Interfaces
{
    public interface IMinioService
    {
        Task<string> UploadFileAsync(IFormFile file, int userId);
        Task<Stream> GetFileAsync(string objectName, int userId);
        Task<bool> DeleteFileAsync(string objectName, int userId);
        Task<List<object>> ListFilesAsync(int userId);
    }
}