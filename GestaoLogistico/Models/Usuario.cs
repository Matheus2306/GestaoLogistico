using GestaoLogistico.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Diagnostics.CodeAnalysis;

namespace GestaoLogistico.Models
{
    public class Usuario : IdentityUser, IAuditavel
    {
        // ✅ Construtor sem parâmetros para o Identity com SetsRequiredMembers
        [SetsRequiredMembers]
        public Usuario()
        {
            NomeCompleto = string.Empty;
            CPF = string.Empty;
        }

        // ✅ Construtor com parâmetros para criação manual
        public Usuario(string nomeCompleto, string cpf, string email)
        {
            NomeCompleto = nomeCompleto;
            CPF = cpf;
            Email = email;
            UserName = email;
        }

        public required string NomeCompleto { get; set; }
        public required string CPF { get; set; }
        public string? UrlFoto { get; set; } = string.Empty;

        // Propriedades de Auditoria
        public DateTime CriadoEm { get; set; }
        public DateTime? AtualizadoEm { get; set; }
        public string? CriadoPorId { get; set; }
        public string? AtualizadoPorId { get; set; }
    }
}
