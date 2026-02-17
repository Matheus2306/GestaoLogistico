using System.ComponentModel.DataAnnotations;

namespace GestaoLogistico.DTOs.UsersDTO
{
    /// <summary>
    /// DTO para atribuir/alterar role de um usuário
    /// </summary>
    public class AssignRoleDTO
    {
        [Required(ErrorMessage = "O ID do usuário é obrigatório.")]
        public required string UserId { get; set; }

        [Required(ErrorMessage = "A role é obrigatória.")]
        public required string Role { get; set; }
    }
}
