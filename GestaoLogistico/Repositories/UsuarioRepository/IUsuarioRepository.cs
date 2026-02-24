using GestaoLogistico.DTOs.UsersDTO;
using GestaoLogistico.Models;

namespace GestaoLogistico.Repositories.UsuarioRepository
{
    /// <summary>
    /// define a interface para o repositório de usuários, que pode incluir métodos para operações CRUD, autenticação, autorização e outras funcionalidades relacionadas aos usuários do sistema.
    /// </summary>
    public interface IUsuarioRepository
    {

        /// <summary>
        /// retorna uma lista de todos os usuários do sistema. Este método pode ser utilizado para exibir informações dos usuários em uma interface administrativa ou para outras finalidades que envolvam a manipulação dos dados dos usuários.
        /// </summary>
        Task<IEnumerable<UserDTOcompleto>> GetAllUsersAsync();

        /// <summary>
        /// retorna as informações básicas do usuário atual autenticado a partir do token JWT.
        /// </summary>
        Task<UserSimpleDTO> GetCurrentUser();

        /// <summary>
        /// Retorna as informações básicas de um usuário específico com base no seu ID. Este método pode ser utilizado para exibir detalhes do usuário em uma interface administrativa ou para outras finalidades que envolvam a manipulação dos dados dos usuários.
        /// </summary>
        Task<Usuario?> GetUserByIdAsync(string userId);

        /// <summary>
        /// Retorna as informações completas de um usuário específico com base no seu email.
        /// </summary>
        Task<Usuario?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Cria um novo usuário no sistema.
        /// </summary>
        Task<(bool Success, IEnumerable<string> Errors, Usuario? User)> CreateUserAsync(Usuario user, string password);

        /// <summary>
        /// Adiciona uma role a um usuário.
        /// </summary>
        Task<bool> AddUserToRoleAsync(Usuario user, string role);

        /// <summary>
        /// Remove uma role de um usuário.
        /// </summary>
        Task<bool> RemoveUserFromRoleAsync(Usuario user, string role);

        /// <summary>
        /// Verifica se uma role existe no sistema.
        /// </summary>
        Task<bool> RoleExistsAsync(string role);

        /// <summary>
        /// Retorna todas as roles disponíveis no sistema.
        /// </summary>
        Task<IEnumerable<string>> GetAllRolesAsync();

        /// <summary>
        /// Atualiza as informações de um usuário no banco de dados.
        /// </summary>
        Task<bool> UpdateUserAsync(Usuario user);

        /// <summary>
        /// Deleta um usuário do banco de dados.
        /// </summary>
        Task<bool> DeleteUserAsync(string userId);

    }
}
