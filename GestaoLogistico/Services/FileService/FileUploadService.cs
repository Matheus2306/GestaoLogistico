namespace GestaoLogistico.Services.FileService
{
    public class FileUploadService : IfileUploadService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileUploadService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> SaveFileAsync(byte[] fileBytes, string fileName, string folder = "uploads")
        {
            try
            {
                if (fileBytes == null || fileBytes.Length == 0)
                {
                    throw new ArgumentException("O arquivo está vazio ou é inválido.");
                }

                if (string.IsNullOrEmpty(_environment.WebRootPath))
                {
                    throw new InvalidOperationException("WebRootPath não está configurado. Certifique-se de que UseStaticFiles() está configurado no Program.cs e que a pasta wwwroot existe.");
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, folder);

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(fileName)}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                await File.WriteAllBytesAsync(filePath, fileBytes);

                var relativePath = $"/{folder}/{uniqueFileName}";

                _logger.LogInformation($"Arquivo salvo com sucesso: {relativePath}");

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao salvar arquivo: {ex.Message}");
                throw new Exception($"Erro ao salvar o arquivo: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteFileAsync(string filePath)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    return false;
                }

                var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

                if (File.Exists(fullPath))
                {
                    await Task.Run(() => File.Delete(fullPath));
                    _logger.LogInformation($"Arquivo deletado com sucesso: {filePath}");
                    return true;
                }

                _logger.LogWarning($"Arquivo não encontrado: {filePath}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Erro ao deletar arquivo: {ex.Message}");
                return false;
            }
        }

        public bool ValidateFileSize(byte[] fileBytes, long maxSizeInMB = 3)
        {
            if (fileBytes == null || fileBytes.Length == 0)
            {
                return false;
            }

            var maxSizeInBytes = maxSizeInMB * 1024 * 1024;
            return fileBytes.Length <= maxSizeInBytes;
        }

        public bool ValidateFileType(string fileName, string[] allowedExtensions)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return false;
            }

            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return allowedExtensions.Contains(extension);
        }

        public string GetFileUrl(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return string.Empty;
            }

            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null) 
                return string.Empty;
            
            var baseUrl = $"{request.Scheme}://{request.Host}";
            var fileUrl = $"{baseUrl}{filePath}";

            return fileUrl;
        }
    }
}
