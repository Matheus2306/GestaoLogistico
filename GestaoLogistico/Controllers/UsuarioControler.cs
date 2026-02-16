using GestaoLogistico.Models;
using GestaoLogistico.Repositories.UsuarioRepository;
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

        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepository.GetAllUsersAsync();
            return Ok(users);
        }
    }
}
