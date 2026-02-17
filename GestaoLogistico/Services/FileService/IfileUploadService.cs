namespace GestaoLogistico.Services.FileService
{
    public interface IfileUploadService
    {
        Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string folder = "uploads");
        Task<bool> DeleteFileAsync(string filePath);
        bool ValidateFileSize(byte[] fileBytes, long maxSizeInMB = 5);
        bool ValidateFileType(string fileName, string[] allowedExtensions);
        string GetFileUrl(string filePath);
    }
}
