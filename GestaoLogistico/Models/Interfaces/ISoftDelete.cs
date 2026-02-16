namespace GestaoLogistico.Models.Interfaces
{
    /// <summary>
    /// Interface para entidades que suportam exclusão lógica (soft delete)
    /// </summary>
    public interface ISoftDelete
    {
        /// <summary>
        /// Indica se o registro foi excluído logicamente
        /// </summary>
        bool Excluido { get; set; }

        /// <summary>
        /// Data e hora da exclusão lógica (UTC)
        /// </summary>
        DateTime? ExcluidoEm { get; set; }

        /// <summary>
        /// ID do usuário que excluiu o registro
        /// </summary>
        string? ExcluidoPorId { get; set; }
    }
}