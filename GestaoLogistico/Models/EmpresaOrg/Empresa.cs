using GestaoLogistico.Models.Empresa;
using GestaoLogistico.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GestaoLogistico.Models.EmpresaOrg
{
    public class Empresa : IAuditavel, ISoftDelete
    {
        [Key]
        public Guid EmpresaId { get; set; }

        [Required]
        [MaxLength(200)]
        public required string RazaoSocial { get; set; }

        [MaxLength(200)]
        public string? NomeFantasia { get; set; }

        [Required]
        [MaxLength(14)]
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

        // Status
        public bool Ativo { get; set; } = true;

        // Relacionamentos
        public ICollection<EmpresaEmail> Emails { get; set; } = new List<EmpresaEmail>();
        public ICollection<EmpresaTelefone> Telefones { get; set; } = new List<EmpresaTelefone>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();

        // Usuário Responsável (proprietário da empresa)
        public string? UsuarioResponsavelId { get; set; }
        public Usuario? UsuarioResponsavel { get; set; }

        // Auditoria
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public string? CriadoPorId { get; set; }
        public string? AtualizadoPorId { get; set; }

        // Soft Delete
        public bool Excluido { get; set; }
        public DateTime? ExcluidoEm { get; set; }
        public string? ExcluidoPorId { get; set; }
    }
}
