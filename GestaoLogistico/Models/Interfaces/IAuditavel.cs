namespace GestaoLogistico.Models.Interfaces
{
    /// <summary>
    /// Interface para entidades que requerem rastreamento de auditoria
    /// </summary>
    public interface IAuditavel
    {
        /// <summary>
        /// Data e hora de criação do registro (UTC)
        /// </summary>
        DateTime CriadoEm { get; set; }

        /// <summary>
        /// Data e hora da última atualização do registro (UTC)
        /// </summary>
        DateTime? AtualizadoEm { get; set; }

        /// <summary>
        /// ID do usuário que criou o registro
        /// </summary>
        string? CriadoPorId { get; set; }

        /// <summary>
        /// ID do usuário que atualizou o registro pela última vez
        /// </summary>
        string? AtualizadoPorId { get; set; }
    }
}