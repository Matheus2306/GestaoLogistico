using GestaoLogistico.DTOs.UsersDTO;

namespace GestaoLogistico.Services.UsuarioService
{
    /// <summary>
    /// Defines the contract for user-related operations within the application.
    /// </summary>
    public interface IUserService
    {
        // ==================== Consultas ====================

        /// <summary>
        /// Asynchronously retrieves a complete list of all users.
        /// </summary>
        /// <remarks>This method may throw exceptions if the operation fails due to network issues or if
        /// user data cannot be retrieved.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of <see cref="UserDTOcompleto"/>
        /// objects with the details of all users.</returns>
        Task<IEnumerable<UserDTOcompleto>> GetAllUsersAsync();

        /// <summary>
        /// retorna as informações do usuário atualmente autenticado no sistema. 
        /// </summary>        
        /// <returns> O DTO do usuário atualmente autenticado</returns>
        Task<UserSimpleDTO> GetCurrentUser();

        // ==================== Fim Consultas ====================
        // ==================== Comandos CRUD ====================

        /// <summary>
        /// Edita as informações de um usuário existente no sistema.
        /// </summary>
        /// <param name="dto">DTO contendo as informações atualizadas do usuário</param>
        /// <returns>O DTO do usuário atualizado</returns>
        Task<UserEditFormDTO> EditUser(UserEditFormDTO dto);

        /// <summary>
        /// Cria um novo usuário vinculado a uma empresa.
        /// </summary>
        /// <param name="dto">DTO contendo as informações do novo usuário</param>
        /// <returns>O DTO completo do usuário criado</returns>
        Task<UserDTOcompleto> CreateUserByCompany(CreateUserByCompanyDTO dto);

        /// <summary>
        /// cria um novo usuário sem vínculo com empresa e sem atribuição de roles.
        /// </summary>
        /// <param name="dto">DTO contendo as informações do novo usuário</param>
        /// <returns>O DTO simples do usuário criado</returns>
        Task<UserSimpleDTO> CreateUserAsync(UserCreateDTO dto);

        // ==================== Fim Comandos CRUD ====================
        // ==================== Gerenciamento de Roles ====================

        /// <summary>
        /// Atribui uma role a um usuário existente.
        /// </summary>
        /// <param name="dto">DTO contendo o ID do usuário e a role a ser atribuída</param>
        /// <returns>True se a operação foi bem-sucedida</returns>
        Task<bool> AssignRoleToUser(AssignRoleDTO dto);

        /// <summary>
        /// Remove uma role de um usuário existente.
        /// </summary>
        /// <param name="dto">DTO contendo o ID do usuário e a role a ser removida</param>
        /// <returns>True se a operação foi bem-sucedida</returns>
        Task<bool> RemoveRoleFromUser(AssignRoleDTO dto);

        /// <summary>
        /// Retorna todas as roles disponíveis no sistema, exceto Administrador.
        /// </summary>
        /// <returns>Lista de roles disponíveis</returns>
        Task<IEnumerable<string>> GetAvailableRoles();

        // ==================== Fim Gerenciamento de Roles ====================
    }
}
