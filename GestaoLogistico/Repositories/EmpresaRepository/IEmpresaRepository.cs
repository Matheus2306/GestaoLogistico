using GestaoLogistico.Models.Empresas;

namespace GestaoLogistico.Repositories.EmpresaRepository
{
    public interface IEmpresaRepository
    {
        /// <summary>
        /// Verifica se um CNPJ já está cadastrado
        /// </summary>
        Task<bool> CNPJExisteAsync(string cnpj);

        /// <summary>
        /// Cria uma nova empresa
        /// </summary>
        Task<Empresa> CriarEmpresaAsync(Empresa empresa);

        /// <summary>
        /// Adiciona emails à empresa
        /// </summary>
        Task AdicionarEmailsAsync(List<EmpresaEmail> emails);

        /// <summary>
        /// Adiciona telefones à empresa
        /// </summary>
        Task AdicionarTelefonesAsync(List<EmpresaTelefone> telefones);

        /// <summary>
        /// Busca empresa completa com relacionamentos
        /// </summary>
        Task<Empresa?> GetEmpresaCompletaAsync(Guid empresaId);

        Task<IEnumerable<Empresa?>> GetAllEmpresaByUser(string UserId);

        Task<bool> DeleteEmpresaAsync(Guid empresaId);
    }
}
