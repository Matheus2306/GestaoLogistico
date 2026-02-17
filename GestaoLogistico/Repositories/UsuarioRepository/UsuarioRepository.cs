using AutoMapper;
using AutoMapper.QueryableExtensions;
using GestaoLogistico.Data;
using GestaoLogistico.DTOs.UsersDTO;
using GestaoLogistico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistico.Repositories.UsuarioRepository
{
    /// <summary>
    /// providencia a implementação concreta do repositório de usuários, que pode incluir métodos para acessar e manipular os dados dos usuários no banco de dados, utilizando o Entity Framework Core ou outra tecnologia de acesso a dados. Essa classe pode ser injetada em controladores ou outros serviços para realizar operações relacionadas aos usuários do sistema.
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioRepository(AplicationDbContext context, IMapper mapper, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<UserDTOcompleto>> GetAllUsersAsync()
        {
            // Primeiro, busca todos os usuários
            var users = await _context.Users.ToListAsync();

            // Depois, mapeia cada usuário e busca suas roles separadamente
            var userDtos = new List<UserDTOcompleto>();
            foreach (var user in users)
            {
                var dto = _mapper.Map<UserDTOcompleto>(user);
                dto.Roles = await _userManager.GetRolesAsync(user);
                userDtos.Add(dto);
            }

            return userDtos;
        }

        public async Task<UserSimpleDTO> GetCurrentUser()
        {
            // Obtém o ID do usuário a partir do token Bearer
            var userId = _httpContextAccessor.HttpContext?.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado."); // Lança uma exceção se o usuário não estiver autenticado
            }

            // Busca o usuário no banco de dados
            var usuario = await _context.Users.FindAsync(userId);

            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuário com ID {userId} não encontrado."); // Lança uma exceção se o usuário não for encontrado
            }

            // Mapeia para DTO
            var userDto = _mapper.Map<UserSimpleDTO>(usuario);

            // Busca as roles do usuário
            userDto.Roles = await _userManager.GetRolesAsync(usuario);

            return userDto;
        }

        public async Task<Usuario?> GetUserByIdAsync(string userId)
        {
            var usuario = await _context.Users.FindAsync(userId);
            return usuario;
        }

        public async Task<Usuario?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<(bool Success, IEnumerable<string> Errors, Usuario? User)> CreateUserAsync(Usuario user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return (true, Enumerable.Empty<string>(), user);
            }

            return (false, result.Errors.Select(e => e.Description), null);
        }

        public async Task<bool> AddUserToRoleAsync(Usuario user, string role)
        {
            var result = await _userManager.AddToRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<bool> RemoveUserFromRoleAsync(Usuario user, string role)
        {
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            return result.Succeeded;
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<IEnumerable<string>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(r => r.Name!).ToListAsync();
        }

        // ============== Metodos CRUD ==============
        public async Task SaveChangesAsync()
        {
            // Salva as alterações no banco de dados
            await _context.SaveChangesAsync();
        }
    }
}
