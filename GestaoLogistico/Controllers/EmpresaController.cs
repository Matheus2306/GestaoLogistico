using GestaoLogistico.DTOs.EmpresaDTO;
using GestaoLogistico.Services.EmpresaService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GestaoLogistico.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresaService _empresaService;
        private readonly ILogger<EmpresaController> _logger;

        public EmpresaController(IEmpresaService empresaService, ILogger<EmpresaController> logger)
        {
            _empresaService = empresaService;
            _logger = logger;
        }

        /// <summary>
        /// Cria uma empresa vinculada ao usuário autenticado.
        /// O usuário se torna automaticamente o responsável e recebe a role "Empresa".
        /// </summary>
        /// <param name="dto">Dados da empresa</param>
        /// <returns>Dados completos da empresa criada</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CriarEmpresa([FromBody] CriarEmpresaDTO dto)
        {
            try
            {
                var result = await _empresaService.CriarEmpresaAsync(dto);
                return CreatedAtAction(nameof(CriarEmpresa), new { id = result.EmpresaId }, result);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning("Tentativa não autorizada: {Message}", ex.Message);
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning("Erro de validação: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar empresa");
                return StatusCode(500, new { message = "Erro interno ao criar empresa.", details = ex.Message });
            }
        }
    }
}
