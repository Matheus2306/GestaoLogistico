using System.ComponentModel.DataAnnotations;

namespace GestaoLogistico.DTOs.UsersDTO
{
    /// <summary>
    /// DTO para criação de usuário por uma empresa
    /// </summary>
    public class CreateUserByCompanyDTO
    {
        [Required(ErrorMessage = "O nome completo é obrigatório.")]
        public required string NomeCompleto { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório.")]
        public required string CPF { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Email inválido.")]
        public required string Email { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória.")]
        [MinLength(6, ErrorMessage = "A senha deve ter no mínimo 6 caracteres.")]
        public required string Password { get; set; }

        [Required(ErrorMessage = "A role é obrigatória.")]
        public required string Role { get; set; }
    }
}
