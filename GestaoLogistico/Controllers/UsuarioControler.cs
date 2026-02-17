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
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }

        [Authorize]
        [HttpGet("Current")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                var user = await _userRepository.GetCurrentUser();
                return Ok(user);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpPut("EditUser")]
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

    }
}
