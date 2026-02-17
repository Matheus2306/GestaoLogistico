using AutoMapper;
using AutoMapper.QueryableExtensions;
using GestaoLogistico.Data;
using GestaoLogistico.DTOs;
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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UsuarioRepository(AplicationDbContext context, IMapper mapper, UserManager<Usuario> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<UserDTOcompleto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    Usuario = u,
                    Roles = _userManager.GetRolesAsync(u).Result // Obtém as roles do usuário
                })
                .ToListAsync();

            var userDtos = users.Select(u =>
            {
                var dto = _mapper.Map<UserDTOcompleto>(u.Usuario);
                dto.Roles = u.Roles;
                return dto;
            }).ToList();

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
    }
}
