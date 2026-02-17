using GestaoLogistico.DTOs.UsersDTO;

namespace GestaoLogistico.Services.UsuarioService
{
    /// <summary>
    /// Defines the contract for user-related operations within the application.
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// Edita as informações de um usuário existente no sistema.
        /// </summary>
        /// <param name="dto">DTO contendo as informações atualizadas do usuário</param>
        /// <returns>O DTO do usuário atualizado</returns>
        Task<UserEditFormDTO> EditUser(UserEditFormDTO dto);
    }
}
