using AutoMapper;
using GestaoLogistico.DTOs.UsersDTO;
using GestaoLogistico.Models;
using GestaoLogistico.Repositories.UsuarioRepository;
using GestaoLogistico.Services.DocValidator;
using GestaoLogistico.Services.FileService;
using GestaoLogistico.Services.FormatService;
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
        private readonly IFormatService _formatService;

        public UserService(ILogger<UserService> logger, IUsuarioRepository usuarioRepository, IMapper mapper, IfileUploadService fileUploadService, IDocValidatorService docValidatorService, IFormatService formatService)
        {
            _logger = logger;
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _fileUploadService = fileUploadService;
            _docValidatorService = docValidatorService;
            _formatService = formatService;
        }

        public async Task<IEnumerable<UserDTOcompleto>> GetAllUsersAsync()
        {
            var users = await _usuarioRepository.GetAllUsersAsync();

            // Converte os caminhos das fotos para URLs completas
            foreach (var user in users)
            {
                user.PhotoUrl = _fileUploadService.GetFileUrl(user.PhotoUrl);
            
                //transforma o horario UTC para o horário local do Brasil
                if (DateTime.TryParse(user.atualizadoEm, out var atualizadoEmUtc))
                {
                    var atualizadoEmLocal = TimeZoneInfo.ConvertTimeFromUtc(atualizadoEmUtc, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
                    user.atualizadoEm = atualizadoEmLocal.ToString("yyyy-MM-dd HH:mm:ss");
                }

                //Adiciona formatação para os campos cpf e telefone
                user.CPF = _formatService.SetupFormatDocument(user.CPF);
                user.PhoneNumber = _formatService.SetupFormatPhone(user.PhoneNumber) ;
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

            //Adiciona formatação para os campos cpf e telefone
            currentUserDto.CPF = _formatService.SetupFormatDocument(currentUserDto.CPF);
            currentUserDto.PhoneNumber = _formatService.SetupFormatPhone(currentUserDto.PhoneNumber);

            return currentUserDto;
        }

        public async Task<IEnumerable<UserSimpleDTO>> GetAllUsuariosByEmpresaIdAsync(Guid EmpresaId)
        {
            var usersLinked = await _usuarioRepository.GetAllUsuariosByEmpresaIdAsync(EmpresaId);
            if (usersLinked == null || !usersLinked.Any())
            {
                _logger.LogWarning("No users found for company ID {CompanyId}.", EmpresaId);
                throw new KeyNotFoundException($"Nenhum usuário encontrado para a empresa.");
            }
            
            foreach (var user in usersLinked)
            {
                user.UrlFoto = _fileUploadService.GetFileUrl(user.UrlFoto);
                //Adiciona formatação para os campos cpf e telefone
                user.CPF = _formatService.SetupFormatDocument(user.CPF);
                user.PhoneNumber = _formatService.SetupFormatPhone(user.PhoneNumber);
            }
            
            var userDTO = _mapper.Map<IEnumerable<UserSimpleDTO>>(usersLinked);

            return userDTO;
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
            if (cpfValidado == null)
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

            // Salvar alterações no usuário
            var success = await _usuarioRepository.UpdateUserAsync(user);
            if (!success)
            {
                _logger.LogError("Failed to update user with ID {UserId}.", user.Id);
                throw new InvalidOperationException("Erro ao atualizar usuário.");
            }

            _logger.LogInformation("User with ID {UserId} updated successfully.", user.Id);

            //setup formatação para os campos cpf e telefone antes de retornar o DTO
            user.PhoneNumber = _formatService.SetupFormatPhone(user.PhoneNumber);
            user.CPF = _formatService.SetupFormatDocument(user.CPF);

            return _mapper.Map<UserEditFormDTO>(user);
        }

        public async Task<UserSimpleDTO> CreateUserAsync(UserCreateDTO dto)
        {
            // Verifica se o email já existe
            var existingUser = await _usuarioRepository.GetUserByEmailAsync(dto.Email);
            if (existingUser != null) 
                throw new ArgumentException("Este email já está cadastrado.");

            // Valida o CPF
            if (_docValidatorService.ValidarCPF(dto.CPF) == null)
            {
                _logger.LogWarning("Invalid CPF provided: {CPF}", dto.CPF);
                throw new ArgumentException("CPF inválido.");
            }


            // Usa o AutoMapper para mapear o DTO para Usuario (usando o construtor parametrizado)
            var newUser = _mapper.Map<Usuario>(dto);
            newUser.EmailConfirmed = false;
            newUser.CriadoEm = DateTime.UtcNow;

            var (success, errors, user) = await _usuarioRepository.CreateUserAsync(newUser, dto.Password);

            if(!success)
            {
                var errorMessages = string.Join(", ", errors);
                _logger.LogWarning("Failed to create user: {Errors}", errorMessages);
                throw new ArgumentException($"Erro ao criar usuário: {errorMessages}");
            }

            _logger.LogInformation("User {UserId} created successfully.", user!.Id);

            //Adiciona formatação para os campos cpf e telefone antes de retornar o DTO
            user.PhoneNumber = _formatService.SetupFormatPhone(user.PhoneNumber);
            user.CPF = _formatService.SetupFormatDocument(user.CPF);

            return _mapper.Map<UserSimpleDTO>(user);
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

            // Busca o usuário completo para obter o EmpresaId
            var currentUserEntity = await _usuarioRepository.GetUserByIdAsync(currentUser.Id);
            if (currentUserEntity?.EmpresaId == null)
            {
                _logger.LogWarning("User {UserId} does not have an associated company.", currentUser.Id);
                throw new UnauthorizedAccessException("Usuário não está vinculado a nenhuma empresa.");
            }

            var empresaId = currentUserEntity.EmpresaId.Value;

            // Verifica se a empresa já atingiu o limite de 10 usuários
            var userCount = await _usuarioRepository.CountUsersByEmpresaAsync(empresaId);
            if (userCount >= 10)
            {
                _logger.LogWarning("Company {CompanyId} has reached the maximum limit of 10 users.", empresaId);
                throw new ArgumentException("Sua empresa já atingiu o limite máximo de 10 usuários.");
            }

            // Valida o CPF
            if (_docValidatorService.ValidarCPF(dto.CPF) == null)
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

            // Usa o AutoMapper para mapear o DTO para Usuario (usando o construtor parametrizado)
            var newUser = _mapper.Map<Usuario>(dto);
            newUser.EmailConfirmed = false;
            newUser.CriadoEm = DateTime.UtcNow;
            newUser.CriadoPorId = currentUser.Id;
            newUser.EmpresaId = empresaId; // Vincula o novo usuário à empresa

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

            _logger.LogInformation("User {UserId} created successfully by company {CompanyId} and linked to the company.", user!.Id, empresaId);


            // Retorna o DTO completo
            var userDto = _mapper.Map<UserDTOcompleto>(user);
            userDto.Roles = new List<string> { dto.Role };
            userDto.CPF = _formatService.SetupFormatDocument(userDto.CPF);

            return userDto;
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var currentUser = await _usuarioRepository.GetCurrentUser();
            if (currentUser == null)
            {
                _logger.LogWarning("Current user not found in token.");
                throw new KeyNotFoundException($"Usuário não foi encontrado.");
            }

            // Busca o usuário a ser deletado
            var targetUser = await _usuarioRepository.GetUserByIdAsync(userId);
            if (targetUser == null)
            {
                _logger.LogWarning("Target user {UserId} not found.", userId);
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            // Se for empresa, verifica se o usuário pertence à mesma empresa
            if (currentUser.Roles.Contains("Empresa"))
            {
                var currentUserEntity = await _usuarioRepository.GetUserByIdAsync(currentUser.Id);
                if (currentUserEntity?.EmpresaId == null)
                {
                    _logger.LogWarning("User {UserId} does not have an associated company.", currentUser.Id);
                    throw new UnauthorizedAccessException("Usuário não está vinculado a nenhuma empresa.");
                }

                if (targetUser.EmpresaId != currentUserEntity.EmpresaId)
                {
                    _logger.LogWarning("User {UserId} tried to delete user {TargetUserId} from different company.", currentUser.Id, userId);
                    throw new UnauthorizedAccessException("Você só pode deletar usuários da sua empresa.");
                }
            }
            // Se não for empresa, permite deletar apenas a própria conta
            else if (currentUser.Id != userId)
            {
                _logger.LogWarning("Unauthorized attempt to delete user {UserId} by user {CurrentUserId}.", userId, currentUser.Id);
                throw new UnauthorizedAccessException("Você só pode deletar sua própria conta.");
            }

            var success = await _usuarioRepository.DeleteUserAsync(userId);
            if (success)
            {
                _logger.LogInformation("User {UserId} deleted successfully by user {CurrentUserId}.", userId, currentUser.Id);
            }
            else
            {
                _logger.LogWarning("Failed to delete user {UserId} by user {CurrentUserId}.", userId, currentUser.Id);
            }
            return success;
        }

        //======================= Gerenciamento de Roles ============================

        public async Task<bool> AssignRoleToUser(AssignRoleDTO dto)
        {
            // Verifica se a empresa está autenticada e tem a role "Empresa"
            var currentUser = await _usuarioRepository.GetCurrentUser();

            if (currentUser == null || !currentUser.Roles.Contains("Empresa"))
            {
                _logger.LogWarning("Unauthorized attempt to assign role by non-company user.");
                throw new UnauthorizedAccessException("Apenas empresas podem atribuir roles.");
            }

            // Busca o usuário completo para obter o EmpresaId
            var currentUserEntity = await _usuarioRepository.GetUserByIdAsync(currentUser.Id);
            if (currentUserEntity?.EmpresaId == null)
            {
                _logger.LogWarning("User {UserId} does not have an associated company.", currentUser.Id);
                throw new UnauthorizedAccessException("Usuário não está vinculado a nenhuma empresa.");
            }

            // Busca o usuário alvo
            var targetUser = await _usuarioRepository.GetUserByIdAsync(dto.UserId);
            if (targetUser == null)
            {
                _logger.LogWarning("User {UserId} not found.", dto.UserId);
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            // Verifica se o usuário alvo pertence à mesma empresa
            if (targetUser.EmpresaId != currentUserEntity.EmpresaId)
            {
                _logger.LogWarning("User {UserId} tried to assign role to user {TargetUserId} from different company.", currentUser.Id, dto.UserId);
                throw new UnauthorizedAccessException("Você só pode atribuir roles a usuários da sua empresa.");
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
                _logger.LogInformation("Role {Role} assigned to user {UserId} by company {CompanyId}", dto.Role, dto.UserId, currentUserEntity.EmpresaId);
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

            // Busca o usuário completo para obter o EmpresaId
            var currentUserEntity = await _usuarioRepository.GetUserByIdAsync(currentUser.Id);
            if (currentUserEntity?.EmpresaId == null)
            {
                _logger.LogWarning("User {UserId} does not have an associated company.", currentUser.Id);
                throw new UnauthorizedAccessException("Usuário não está vinculado a nenhuma empresa.");
            }

            // Busca o usuário alvo
            var targetUser = await _usuarioRepository.GetUserByIdAsync(dto.UserId);
            if (targetUser == null)
            {
                _logger.LogWarning("User {UserId} not found.", dto.UserId);
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            // Verifica se o usuário alvo pertence à mesma empresa
            if (targetUser.EmpresaId != currentUserEntity.EmpresaId)
            {
                _logger.LogWarning("User {UserId} tried to remove role from user {TargetUserId} from different company.", currentUser.Id, dto.UserId);
                throw new UnauthorizedAccessException("Você só pode remover roles de usuários da sua empresa.");
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
                _logger.LogInformation("Role {Role} removed from user {UserId} by company {CompanyId}", dto.Role, dto.UserId, currentUserEntity.EmpresaId);
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

        public async Task<int> GetCompanyUserCount()
        {
            // Verifica se a empresa está autenticada e tem a role "Empresa"
            var currentUser = await _usuarioRepository.GetCurrentUser();

            if (currentUser == null || !currentUser.Roles.Contains("Empresa"))
            {
                _logger.LogWarning("Unauthorized attempt to get user count by non-company user.");
                throw new UnauthorizedAccessException("Apenas empresas podem consultar a quantidade de usuários.");
            }

            // Busca o usuário completo para obter o EmpresaId
            var currentUserEntity = await _usuarioRepository.GetUserByIdAsync(currentUser.Id);
            if (currentUserEntity?.EmpresaId == null)
            {
                _logger.LogWarning("User {UserId} does not have an associated company.", currentUser.Id);
                throw new UnauthorizedAccessException("Usuário não está vinculado a nenhuma empresa.");
            }

            var count = await _usuarioRepository.CountUsersByEmpresaAsync(currentUserEntity.EmpresaId.Value);
            _logger.LogInformation("Company {CompanyId} has {Count} users.", currentUserEntity.EmpresaId, count);

            return count;
        }


    }
}
