using GestaoLogistico.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GestaoLogistico.Models.Empresa
{
    public class EmpresaEmail : IAuditavel, ISoftDelete
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmpresaId { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public required string Email { get; set; }

        [MaxLength(50)]
        public string? Tipo { get; set; } // Ex: Comercial, Financeiro, Suporte, Geral

        public bool Principal { get; set; } = false;

        // Relacionamento
        public Empresa Empresa { get; set; } = null!;

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
