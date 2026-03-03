using System.ComponentModel.DataAnnotations;

namespace GestaoLogistico.DTOs.EmpresaDTO
{
    /// <summary>
    /// DTO para criação de empresa por usuário autenticado
    /// </summary>
    public class CriarEmpresaDTO
    {
        // Dados da Empresa
        [Required(ErrorMessage = "A razão social é obrigatória.")]
        [MaxLength(200)]
        public required string RazaoSocial { get; set; }

        [MaxLength(200)]
        public string? NomeFantasia { get; set; }

        [Required(ErrorMessage = "O CNPJ é obrigatório.")]
        [MaxLength(18)]
        public required string CNPJ { get; set; }

        [MaxLength(20)]
        public string? InscricaoEstadual { get; set; }

        [MaxLength(20)]
        public string? InscricaoMunicipal { get; set; }

        // Endereço
        [MaxLength(10)]
        public string? CEP { get; set; }

        [MaxLength(200)]
        public string? Logradouro { get; set; }

        [MaxLength(20)]
        public string? Numero { get; set; }

        [MaxLength(100)]
        public string? Complemento { get; set; }

        [MaxLength(100)]
        public string? Bairro { get; set; }

        [MaxLength(100)]
        public string? Cidade { get; set; }

        [MaxLength(2)]
        public string? UF { get; set; }

        // Emails e Telefones
        public List<EmpresaEmailDTO>? Emails { get; set; }
        public List<EmpresaTelefoneDTO>? Telefones { get; set; }
    }

    /// <summary>
    /// DTO para edição de empresa - Apenas os campos fornecidos serão atualizados
    /// O ID da empresa vem da rota da URL
    /// </summary>
    public class EmpresaEditDTO
    {
        // Dados da Empresa (todos opcionais para permitir atualização parcial)
        [MaxLength(200)]
        public string? RazaoSocial { get; set; }

        [MaxLength(200)]
        public string? NomeFantasia { get; set; }

        [MaxLength(18)]
        public string? CNPJ { get; set; }

        [MaxLength(20)]
        public string? InscricaoEstadual { get; set; }

        [MaxLength(20)]
        public string? InscricaoMunicipal { get; set; }

        // Endereço
        [MaxLength(10)]
        public string? CEP { get; set; }

        [MaxLength(200)]
        public string? Logradouro { get; set; }

        [MaxLength(20)]
        public string? Numero { get; set; }

        [MaxLength(100)]
        public string? Complemento { get; set; }

        [MaxLength(100)]
        public string? Bairro { get; set; }

        [MaxLength(100)]
        public string? Cidade { get; set; }

        [MaxLength(2)]
        public string? UF { get; set; }

        // Emails e Telefones (opcionais)
        public List<EmpresaEmailDTO>? Emails { get; set; }
        public List<EmpresaTelefoneDTO>? Telefones { get; set; }
    }

    public class EmpresaEmailDTO
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        public string? Tipo { get; set; }
        public bool Principal { get; set; } = false;
    }

    public class EmpresaTelefoneDTO
    {
        [Required]
        public required string Numero { get; set; }

        public string? Tipo { get; set; }
        public bool Principal { get; set; } = false;
        public bool WhatsApp { get; set; } = false;
    }
}
