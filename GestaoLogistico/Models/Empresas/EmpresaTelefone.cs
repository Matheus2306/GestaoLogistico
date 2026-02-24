using GestaoLogistico.Models.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace GestaoLogistico.Models.Empresas
{
    public class EmpresaTelefone : IAuditavel, ISoftDelete
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid EmpresaId { get; set; }

        [Required]
        [MaxLength(20)]
        public required string Numero { get; set; }

        [MaxLength(50)]
        public string? Tipo { get; set; } // Ex: Comercial, Financeiro, Suporte, WhatsApp, Celular, Fixo

        public bool Principal { get; set; } = false;

        public bool WhatsApp { get; set; } = false;

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
