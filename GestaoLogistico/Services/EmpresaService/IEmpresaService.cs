using GestaoLogistico.DTOs.EmpresaDTO;

namespace GestaoLogistico.Services.EmpresaService
{
    public interface IEmpresaService
    {
        /// <summary>
        /// Cria uma empresa vinculada ao usuário autenticado
        /// </summary>
        /// <param name="dto">Dados da empresa</param>
        /// <returns>Dados completos da empresa criada</returns>
        Task<EmpresaDTOCompleto> CriarEmpresaAsync(CriarEmpresaDTO dto);
    }
}
