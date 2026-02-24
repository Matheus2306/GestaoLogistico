using GestaoLogistico.Data;
using GestaoLogistico.Models.Empresas;
using Microsoft.EntityFrameworkCore;

namespace GestaoLogistico.Repositories.EmpresaRepository
{
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly AplicationDbContext _context;

        public EmpresaRepository(AplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> CNPJExisteAsync(string cnpj)
        {
            return await _context.Empresas.AnyAsync(e => e.CNPJ == cnpj);
        }

        public async Task<Empresa> CriarEmpresaAsync(Empresa empresa)
        {
            _context.Empresas.Add(empresa);
            await _context.SaveChangesAsync();
            return empresa;
        }

        public async Task AdicionarEmailsAsync(List<EmpresaEmail> emails)
        {
            _context.EmpresaEmails.AddRange(emails);
            await _context.SaveChangesAsync();
        }

        public async Task AdicionarTelefonesAsync(List<EmpresaTelefone> telefones)
        {
            _context.EmpresaTelefones.AddRange(telefones);
            await _context.SaveChangesAsync();
        }

        public async Task<Empresa?> GetEmpresaCompletaAsync(Guid empresaId)
        {
            return await _context.Empresas
                .Include(e => e.Emails)
                .Include(e => e.Telefones)
                .Include(e => e.UsuarioResponsavel)
                .FirstOrDefaultAsync(e => e.EmpresaId == empresaId);
        }

        /// Método para obter a empresa associada a um usuário específico
        public async Task<IEnumerable<Empresa?>> GetAllEmpresaByUser(string UserId)
        {
            return await _context.Empresas
                .Include(e => e.Usuarios)
                .Where(e => e.Usuarios.Any(u => u.Id == UserId))
                .ToListAsync();
        }
    }
}
