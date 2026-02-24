using System.ComponentModel.DataAnnotations;

namespace GestaoLogistico.DTOs.UsersDTO
{
    public class UserCreateDTO
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "O nome deve ter entre 3 e 100 caracteres")]
        public string? Nome { get; set; }

        [Required(ErrorMessage = "O CPF é obrigatório")]
        [RegularExpression(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$|^\d{11}$", ErrorMessage = "CPF inválido. Use o formato: 000.000.000-00 ou 00000000000")]
        public string? CPF { get; set; }

        [Required(ErrorMessage = "O email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "O email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "A senha é obrigatória")]
        [StringLength(12, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 12 caracteres")]
        public string? Password { get; set; }

        [Phone(ErrorMessage = "Número de telefone inválido")]
        [StringLength(20, ErrorMessage = "O telefone deve ter no máximo 20 caracteres")]
        public string? PhoneNumber { get; set; } 
    }
}
