using AutoMapper;
using GestaoLogistico.DTOs.UsersDTO;
using GestaoLogistico.Repositories.UsuarioRepository;
using GestaoLogistico.Services.DocValidator;
using GestaoLogistico.Services.FileService;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistico.Services.UsuarioService
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
        private readonly IfileUploadService _fileUploadService;
        private readonly IDocValidatorService _docValidatorService;

        public UserService(ILogger<UserService> logger, IUsuarioRepository usuarioRepository, IMapper mapper, IfileUploadService fileUploadService, IDocValidatorService docValidatorService)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
            _docValidatorService = docValidatorService;
        }

        //============ CRUD ===============
        public async Task<UserEditFormDTO> EditUser(UserEditFormDTO dto)
        {
            var currentUserDto = await _usuarioRepository.GetCurrentUser();

            if (currentUserDto == null)
            {
                _logger.LogWarning("Current user not found in token.");
                throw new KeyNotFoundException($"Usuário não foi encontrado.");
            }

            var user = await _usuarioRepository.GetUserByIdAsync(currentUserDto.Id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {UserId} not found.", currentUserDto.Id);
                throw new KeyNotFoundException($"Usuário não foi encontrado.");
            }

            //verifica se o CPF é válido
            var cpfValidado = _docValidatorService.ValidarCPF(dto.CPF);
            if (!cpfValidado)
            {
                _logger.LogWarning("Invalid CPF provided for user ID {UserId}.", currentUserDto.Id);
                throw new ArgumentException("CPF inválido.");
            }

            // Aplicar valores padrão se não fornecidos
            dto.Nome ??= currentUserDto.Nome;
            dto.CPF ??= currentUserDto.CPF;
            dto.Email ??= currentUserDto.Email;
            dto.PhoneNumber ??= currentUserDto.PhoneNumber;

            // Processar upload de foto se fornecida
            if (dto.PhotoFile != null && dto.PhotoFile.Length > 0)
            {

                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileName = $"user_{user.Id}{Path.GetExtension(dto.PhotoFile.FileName)}";

                // Converter IFormFile para byte array
                using var memoryStream = new MemoryStream();
                await dto.PhotoFile.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                if (!_fileUploadService.ValidateFileSize(fileBytes, 3))
                {
                    throw new ArgumentException("O arquivo excede o tamanho máximo de 3MB.");
                }

                if (!_fileUploadService.ValidateFileType(dto.PhotoFile.FileName, allowedExtensions))
                {
                    throw new ArgumentException("Tipo de arquivo não permitido. Use jpg, jpeg, png, gif ou webp.");
                }

                // Deletar foto anterior se existir
                if (!string.IsNullOrEmpty(user.UrlFoto))
                {
                    await _fileUploadService.DeleteFileAsync(user.UrlFoto);
                }

                var filePath = await _fileUploadService.SaveFileAsync(fileBytes, fileName, "uploads/users");
                user.UrlFoto = filePath;
            }

            // Aplicar o mapeamento do DTO para o usuário
            _mapper.Map(dto, user);

            await _usuarioRepository.SaveChangesAsync();

            _logger.LogInformation("User with ID {UserId} updated successfully.", user.Id);

            return _mapper.Map<UserEditFormDTO>(user);
        }
    }
}
