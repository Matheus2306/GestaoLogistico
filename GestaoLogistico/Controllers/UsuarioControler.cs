using GestaoLogistico.Models;
using GestaoLogistico.Repositories.UsuarioRepository;
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

        public UsuarioControler(UserManager<Usuario> userManager, IUsuarioRepository UserRepository)
        {
            _userManager = userManager;
            _userRepository = UserRepository;
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

    }
}
