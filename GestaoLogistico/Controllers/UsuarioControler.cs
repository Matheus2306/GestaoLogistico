using GestaoLogistico.DTOs.UsersDTO;
using GestaoLogistico.Models;
using GestaoLogistico.Repositories.UsuarioRepository;
using GestaoLogistico.Services.UsuarioService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuarioControler : ControllerBase
    {
        private readonly UserManager<Usuario> _userManager;
        private readonly IUsuarioRepository _userRepository;
        private readonly IUserService _userService;

        public UsuarioControler(UserManager<Usuario> userManager, IUsuarioRepository UserRepository, IUserService userService)
        {
            _userManager = userManager;
            _userRepository = UserRepository;
            _userService = userService;
        }

        [Authorize(Roles = "Administrador")]
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("Current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _userService.GetCurrentUser();
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }


        // ==================== OPERAÇÕES CRUD ====================
        [Authorize]
        [HttpPut("EditUser/{id}")]
        public async Task<IActionResult> EditUser([FromForm] UserEditFormDTO formDto)
        {
            try
            {
                var result = await _userService.EditUser(formDto);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CreateUser")]
        public async Task<IActionResult> CreateUser([FromBody] UserCreateDTO dto)
        {
            try
            {
                var result = await _userService.CreateUserAsync(dto);
                return CreatedAtAction(nameof(GetAllUsers), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao criar usuário.", details = ex.Message });
            }
        }

        // ==================== FIM OPERAÇÕES CRUD ====================

        //===================== OPERAÇÕES DE VINCULO DE USUÁRIO A EMPRESA =========================

        /// <summary>
        /// Cria um novo usuário vinculado à empresa autenticada
        /// </summary>
        [Authorize(Roles = "Empresa")]
        [HttpPost("CreateUserByCompany")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserByCompanyDTO dto)
        {
            try
            {
                var result = await _userService.CreateUserByCompany(dto);
                return CreatedAtAction(nameof(GetAllUsers), new { id = result.Id }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao criar usuário.", details = ex.Message });
            }
        }

        /// <summary>
        /// Atribui uma role a um usuário existente
        /// </summary>
        [Authorize(Roles = "Empresa")]
        [HttpPost("AssignRole")]
        public async Task<IActionResult> AssignRole([FromBody] AssignRoleDTO dto)
        {
            try
            {
                var result = await _userService.AssignRoleToUser(dto);
                if (result)
                {
                    return Ok(new { message = "Role atribuída com sucesso." });
                }
                return BadRequest(new { message = "Falha ao atribuir role." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao atribuir role.", details = ex.Message });
            }
        }

        /// <summary>
        /// Remove uma role de um usuário existente
        /// </summary>
        [Authorize(Roles = "Empresa")]
        [HttpPost("RemoveRole")]
        public async Task<IActionResult> RemoveRole([FromBody] AssignRoleDTO dto)
        {
            try
            {
                var result = await _userService.RemoveRoleFromUser(dto);
                if (result)
                {
                    return Ok(new { message = "Role removida com sucesso." });
                }
                return BadRequest(new { message = "Falha ao remover role." });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao remover role.", details = ex.Message });
            }
        }

        /// <summary>
        /// Retorna todas as roles disponíveis para atribuição (exceto Administrador)
        /// </summary>
        [Authorize(Roles = "Empresa")]
        [HttpGet("AvailableRoles")]
        public async Task<IActionResult> GetAvailableRoles()
        {
            try
            {
                var roles = await _userService.GetAvailableRoles();
                return Ok(roles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Erro interno ao buscar roles.", details = ex.Message });
            }
        }
    }
}
