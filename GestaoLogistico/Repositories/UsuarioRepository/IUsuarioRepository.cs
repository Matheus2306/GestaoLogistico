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
        /// salva as alterações feitas no contexto do banco de dados.
        /// </summary>
        Task SaveChangesAsync();

    }
}
