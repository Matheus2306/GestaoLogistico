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

        public UsuarioRepository(AplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UserDTOcompleto>> GetAllUsersAsync()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    Usuario = u,
                    Roles = _context.UserRoles
                        .Where(ur => ur.UserId == u.Id)
                        .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                        .ToList()
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
    }
}
