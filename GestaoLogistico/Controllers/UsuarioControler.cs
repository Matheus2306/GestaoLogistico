using GestaoLogistico.Models;
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

        public UsuarioControler(UserManager<Usuario> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> GetAllUsers()
        {
            return Ok();
        }
    }
}
