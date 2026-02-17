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

        public async Task<IEnumerable<UserDTOcompleto>> GetAllUsersAsync()
        {
            var users = await _usuarioRepository.GetAllUsersAsync();

            // Converte os caminhos das fotos para URLs completas
            foreach (var user in users)
            {
                user.PhotoUrl = _fileUploadService.GetFileUrl(user.PhotoUrl);
            }

            return users;
        }

        public async Task<UserSimpleDTO> GetCurrentUser()
        {
            var currentUserDto = await _usuarioRepository.GetCurrentUser();
            if (currentUserDto == null)
            {
                _logger.LogWarning("Current user not found in token.");
                throw new KeyNotFoundException($"Usuário não foi encontrado.");
            }

            //converter o caminho da foto para uma URL completa
            currentUserDto.UrlPhoto = _fileUploadService.GetFileUrl(currentUserDto.UrlPhoto);

            return currentUserDto;
        }

        //======================= CRUD ============================
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

        public async Task<UserDTOcompleto> CreateUserByCompany(CreateUserByCompanyDTO dto)
        {
            // Verifica se a empresa está autenticada e tem a role "Empresa"
            var currentUser = await _usuarioRepository.GetCurrentUser();

            if (currentUser == null || !currentUser.Roles.Contains("Empresa"))
            {
                _logger.LogWarning("Unauthorized attempt to create user by non-company user.");
                throw new UnauthorizedAccessException("Apenas empresas podem criar usuários.");
            }

            // Valida o CPF
            if (!_docValidatorService.ValidarCPF(dto.CPF))
            {
                _logger.LogWarning("Invalid CPF provided: {CPF}", dto.CPF);
                throw new ArgumentException("CPF inválido.");
            }

            // Verifica se o email já existe
            var existingUser = await _usuarioRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Email already exists: {Email}", dto.Email);
                throw new ArgumentException("Este email já está cadastrado.");
            }

            // Valida a role (não permite criar Administrador)
            var allowedRoles = new[] { "Empresa", "Gestor Logístico", "Operador Logístico" };
            if (!allowedRoles.Contains(dto.Role))
            {
                _logger.LogWarning("Invalid role provided: {Role}", dto.Role);
                throw new ArgumentException("Role inválida. Use: Empresa, Gestor Logístico ou Operador Logístico.");
            }

            // Verifica se a role existe
            if (!await _usuarioRepository.RoleExistsAsync(dto.Role))
            {
                _logger.LogWarning("Role does not exist: {Role}", dto.Role);
                throw new ArgumentException($"A role '{dto.Role}' não existe no sistema.");
            }

            // Cria o novo usuário
            var newUser = new Models.Usuario
            {
                UserName = dto.Email,
                Email = dto.Email,
                NomeCompleto = dto.NomeCompleto,
                CPF = dto.CPF,
                PhoneNumber = dto.PhoneNumber,
                EmailConfirmed = false,
                CriadoEm = DateTime.UtcNow,
                CriadoPorId = currentUser.Id
            };

            var (success, errors, user) = await _usuarioRepository.CreateUserAsync(newUser, dto.Password);

            if (!success)
            {
                var errorMessages = string.Join(", ", errors);
                _logger.LogWarning("Failed to create user: {Errors}", errorMessages);
                throw new ArgumentException($"Erro ao criar usuário: {errorMessages}");
            }

            // Adiciona a role ao usuário
            var roleAdded = await _usuarioRepository.AddUserToRoleAsync(user!, dto.Role);
            if (!roleAdded)
            {
                _logger.LogWarning("Failed to add role {Role} to user {UserId}", dto.Role, user!.Id);
                throw new ArgumentException($"Erro ao atribuir a role '{dto.Role}' ao usuário.");
            }

            _logger.LogInformation("User {UserId} created successfully by company {CompanyId}", user!.Id, currentUser.Id);

            // Retorna o DTO completo
            var userDto = _mapper.Map<UserDTOcompleto>(user);
            userDto.Roles = new List<string> { dto.Role };

            return userDto;
        }

        public async Task<bool> AssignRoleToUser(AssignRoleDTO dto)
        {
            // Verifica se a empresa está autenticada e tem a role "Empresa"
            var currentUser = await _usuarioRepository.GetCurrentUser();

            if (currentUser == null || !currentUser.Roles.Contains("Empresa"))
            {
                _logger.LogWarning("Unauthorized attempt to assign role by non-company user.");
                throw new UnauthorizedAccessException("Apenas empresas podem atribuir roles.");
            }

            // Busca o usuário alvo
            var targetUser = await _usuarioRepository.GetUserByIdAsync(dto.UserId);
            if (targetUser == null)
            {
                _logger.LogWarning("User {UserId} not found.", dto.UserId);
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            // Valida a role (não permite atribuir Administrador)
            var allowedRoles = new[] { "Empresa", "Gestor Logístico", "Operador Logístico" };
            if (!allowedRoles.Contains(dto.Role))
            {
                _logger.LogWarning("Invalid role provided: {Role}", dto.Role);
                throw new ArgumentException("Role inválida. Use: Empresa, Gestor Logístico ou Operador Logístico.");
            }

            // Verifica se a role existe
            if (!await _usuarioRepository.RoleExistsAsync(dto.Role))
            {
                _logger.LogWarning("Role does not exist: {Role}", dto.Role);
                throw new ArgumentException($"A role '{dto.Role}' não existe no sistema.");
            }

            var success = await _usuarioRepository.AddUserToRoleAsync(targetUser, dto.Role);

            if (success)
            {
                _logger.LogInformation("Role {Role} assigned to user {UserId} by company {CompanyId}", dto.Role, dto.UserId, currentUser.Id);
            }
            else
            {
                _logger.LogWarning("Failed to assign role {Role} to user {UserId}", dto.Role, dto.UserId);
            }

            return success;
        }

        public async Task<bool> RemoveRoleFromUser(AssignRoleDTO dto)
        {
            // Verifica se a empresa está autenticada e tem a role "Empresa"
            var currentUser = await _usuarioRepository.GetCurrentUser();

            if (currentUser == null || !currentUser.Roles.Contains("Empresa"))
            {
                _logger.LogWarning("Unauthorized attempt to remove role by non-company user.");
                throw new UnauthorizedAccessException("Apenas empresas podem remover roles.");
            }

            // Busca o usuário alvo
            var targetUser = await _usuarioRepository.GetUserByIdAsync(dto.UserId);
            if (targetUser == null)
            {
                _logger.LogWarning("User {UserId} not found.", dto.UserId);
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            // Não permite remover role Administrador
            if (dto.Role == "Administrador")
            {
                _logger.LogWarning("Attempt to remove Administrador role.");
                throw new UnauthorizedAccessException("Não é permitido remover a role Administrador.");
            }

            var success = await _usuarioRepository.RemoveUserFromRoleAsync(targetUser, dto.Role);

            if (success)
            {
                _logger.LogInformation("Role {Role} removed from user {UserId} by company {CompanyId}", dto.Role, dto.UserId, currentUser.Id);
            }
            else
            {
                _logger.LogWarning("Failed to remove role {Role} from user {UserId}", dto.Role, dto.UserId);
            }

            return success;
        }

        public async Task<IEnumerable<string>> GetAvailableRoles()
        {
            var allRoles = await _usuarioRepository.GetAllRolesAsync();
            // Remove "Administrador" das roles disponíveis
            return allRoles.Where(r => r != "Administrador");
        }


    }
}
