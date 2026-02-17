using AutoMapper;
using GestaoLogistico.DTOs.EmpresaDTO;
using GestaoLogistico.Models;
using GestaoLogistico.Models.Empresa;
using GestaoLogistico.Repositories.EmpresaRepository;
using GestaoLogistico.Repositories.UsuarioRepository;
using GestaoLogistico.Services.DocValidator;
using Microsoft.AspNetCore.Identity;

namespace GestaoLogistico.Services.EmpresaService
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;
        private readonly IDocValidatorService _docValidatorService;
        private readonly ILogger<EmpresaService> _logger;

        public EmpresaService(
            IEmpresaRepository empresaRepository,
            IUsuarioRepository usuarioRepository,
            UserManager<Usuario> userManager,
            IMapper mapper,
            IDocValidatorService docValidatorService,
            ILogger<EmpresaService> logger)
        {
            _empresaRepository = empresaRepository;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
            _mapper = mapper;
            _docValidatorService = docValidatorService;
            _logger = logger;
        }

        public async Task<EmpresaDTOCompleto> CriarEmpresaAsync(CriarEmpresaDTO dto)
        {
            // 1. Obter usuário autenticado
            var currentUser = await _usuarioRepository.GetCurrentUser();
            if (currentUser == null)
            {
                _logger.LogWarning("Tentativa de criar empresa sem estar autenticado.");
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            // 2. Validar CNPJ
            if (!_docValidatorService.ValidarCNPJ(dto.CNPJ))
            {
                _logger.LogWarning("CNPJ inválido fornecido: {CNPJ} por usuário {UserId}", dto.CNPJ, currentUser.Id);
                throw new ArgumentException("CNPJ inválido.");
            }

            // 3. Verificar se o CNPJ já está cadastrado
            if (await _empresaRepository.CNPJExisteAsync(dto.CNPJ))
            {
                _logger.LogWarning("CNPJ já cadastrado: {CNPJ}", dto.CNPJ);
                throw new ArgumentException("Este CNPJ já está cadastrado.");
            }

            // 4. Buscar entidade completa do usuário
            var usuario = await _usuarioRepository.GetUserByIdAsync(currentUser.Id);
            if (usuario == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            // 5. Criar empresa usando AutoMapper
            var empresa = _mapper.Map<Empresa>(dto);
            empresa.EmpresaId = Guid.NewGuid();
            empresa.Ativo = true;
            empresa.UsuarioResponsavelId = usuario.Id;
            empresa.CriadoEm = DateTime.UtcNow;
            empresa.CriadoPorId = usuario.Id;

            await _empresaRepository.CriarEmpresaAsync(empresa);

            // 6. Vincular usuário à empresa
            usuario.EmpresaId = empresa.EmpresaId;
            await _usuarioRepository.SaveChangesAsync();

            // 7. Atribuir role "Empresa" se não tiver
            if (!await _userManager.IsInRoleAsync(usuario, "Empresa"))
            {
                await _userManager.AddToRoleAsync(usuario, "Empresa");
                _logger.LogInformation("Role 'Empresa' atribuída ao usuário {UserId}", usuario.Id);
            }

            // 8. Adicionar emails (se fornecidos)
            if (dto.Emails != null && dto.Emails.Any())
            {
                var emails = dto.Emails.Select(e => new EmpresaEmail
                {
                    Id = Guid.NewGuid(),
                    EmpresaId = empresa.EmpresaId,
                    Email = e.Email,
                    Tipo = e.Tipo,
                    Principal = e.Principal,
                    CriadoEm = DateTime.UtcNow,
                    CriadoPorId = usuario.Id
                }).ToList();

                await _empresaRepository.AdicionarEmailsAsync(emails);
            }

            // 9. Adicionar telefones (se fornecidos)
            if (dto.Telefones != null && dto.Telefones.Any())
            {
                var telefones = dto.Telefones.Select(t => new EmpresaTelefone
                {
                    Id = Guid.NewGuid(),
                    EmpresaId = empresa.EmpresaId,
                    Numero = t.Numero,
                    Tipo = t.Tipo,
                    Principal = t.Principal,
                    WhatsApp = t.WhatsApp,
                    CriadoEm = DateTime.UtcNow,
                    CriadoPorId = usuario.Id
                }).ToList();

                await _empresaRepository.AdicionarTelefonesAsync(telefones);
            }

            _logger.LogInformation("Empresa {EmpresaId} criada com sucesso pelo usuário {UserId}",
                empresa.EmpresaId, usuario.Id);

            // 10. Buscar empresa completa e retornar DTO
            var empresaCompleta = await _empresaRepository.GetEmpresaCompletaAsync(empresa.EmpresaId);
            return _mapper.Map<EmpresaDTOCompleto>(empresaCompleta);
        }
    }
}
