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
        Task<EmpresaSimpleDTO> CriarEmpresaAsync(CriarEmpresaDTO dto);

        /// <summary>
        /// Retrieves the simple representation of the company associated with the current user.
        /// </summary>
        /// <remarks>This method is asynchronous and should be awaited. It may throw exceptions if the
        /// user is not associated with any company or if there are issues retrieving the data.</remarks>
        /// <returns>A task that represents the asynchronous operation, containing an instance of EmpresaSimpleDTO that holds the
        /// company information for the user.</returns>
        Task<IEnumerable<EmpresaSimpleDTO>> GetEmpresaByUserAsync();

        /// <summary>
        /// Atualiza os dados de uma empresa existente. O usuário deve ser o responsável pela empresa.
        /// </summary>
        /// <param name="empresaId">ID da empresa a ser atualizada</param>
        /// <param name="dto">Dados atualizados da empresa</param>
        /// <returns>Dados completos da empresa atualizada</returns>
        Task<EmpresaSimpleDTO> UpdateEmpresaAsync(Guid empresaId, EmpresaEditDTO dto);

        /// <summary>
        /// deleta uma empresa pelo seu ID. O usuário deve ser o responsável pela empresa para realizar a exclusão.
        /// </summary>
        Task<bool> DeleteEmpresaAsync(Guid empresaId);
    }
}
