using AutoMapper;
using GestaoLogistico.DTOs.EmpresaDTO;
using GestaoLogistico.DTOs.UsersDTO;
using GestaoLogistico.Models;
using GestaoLogistico.Models.Empresas;
using GestaoLogistico.Repositories.EmpresaRepository;
using GestaoLogistico.Repositories.UsuarioRepository;
using GestaoLogistico.Services.DocValidator;
using GestaoLogistico.Services.FormatService;
using GestaoLogistico.Services.UsuarioService;
using GestaoLogistico.Services.Validator.Phone;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace GestaoLogistico.Services.EmpresaService
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IUserService _userService;
        private readonly UserManager<Usuario> _userManager;
        private readonly IMapper _mapper;
        private readonly IDocValidatorService _docValidatorService;
        private readonly IFormatService _formatService;
        private readonly ILogger<EmpresaService> _logger;
        private readonly IPhoneValidator _phoneValidator;

        public EmpresaService(
            IEmpresaRepository empresaRepository,
            IUsuarioRepository usuarioRepository,
            UserManager<Usuario> userManager,
            IMapper mapper,
            IDocValidatorService docValidatorService,
            ILogger<EmpresaService> logger,
            IFormatService formatService,
            IPhoneValidator phoneValidator,
            IUserService userService)
        {
            _empresaRepository = empresaRepository;
            _usuarioRepository = usuarioRepository;
            _userManager = userManager;
            _mapper = mapper;
            _docValidatorService = docValidatorService;
            _formatService = formatService;
            _logger = logger;
            _phoneValidator = phoneValidator;
            _userService = userService;
        }

        public async Task<EmpresaSimpleDTO> CriarEmpresaAsync(CriarEmpresaDTO dto)
        {
            // 1. Obter usuário autenticado
            var currentUser = await _usuarioRepository.GetCurrentUser();
            if (currentUser == null)
            {
                _logger.LogWarning("Tentativa de criar empresa sem estar autenticado.");
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            // 2. Validar e limpar CNPJ (remover formatação)
            var cnpjLimpo = _docValidatorService.ValidarCNPJ(dto.CNPJ);
            if (cnpjLimpo == null)
            {
                _logger.LogWarning("CNPJ inválido fornecido: {CNPJ} por usuário {UserId}", dto.CNPJ, currentUser.Id);
                throw new ArgumentException("CNPJ inválido.");
            }

            // 3. Verificar se o CNPJ já está cadastrado (usando o CNPJ limpo)
            if (await _empresaRepository.CNPJExisteAsync(cnpjLimpo))
            {
                _logger.LogWarning("CNPJ já cadastrado: {CNPJ}", cnpjLimpo);
                throw new ArgumentException("Este CNPJ já está cadastrado.");
            }

            // 4. Buscar entidade completa do usuário
            var usuario = await _usuarioRepository.GetUserByIdAsync(currentUser.Id);
            if (usuario == null)
            {
                throw new KeyNotFoundException("Usuário não encontrado.");
            }

            // 5. Mapear empresa usando AutoMapper
            var empresa = _mapper.Map<Empresa>(dto);
            empresa.CNPJ = cnpjLimpo; // Usar o CNPJ limpo (sem formatação)
            empresa.UsuarioResponsavelId = usuario.Id;
            empresa.CriadoEm = DateTime.UtcNow;
            empresa.CriadoPorId = usuario.Id;

            // 6. Verificar se o usuário já tem uma empresa vinculada e logar aviso se tiver
            if (usuario.EmpresaId != null)
            {
                _logger.LogWarning("Usuário {UserId} já está vinculado a outra empresa {EmpresaIdAnterior}. Vinculando à nova empresa {EmpresaIdNova}.",
                    usuario.Id, usuario.EmpresaId, empresa.EmpresaId);
                throw new InvalidOperationException("Usuário já está vinculado a outra empresa. Por favor, desvincule-se da empresa atual antes de criar uma nova."); 
            }

            // 7. Preparar emails
            if(dto.Emails == null)
                throw new ArgumentException("A empresa deve conter pelo menos um email de contato.");

            // 7. Preparar emails
            if(dto.Emails == null)
                throw new ArgumentException("A empresa deve conter pelo menos um email de contato.");

            var emails = _mapper.Map<List<EmpresaEmail>>(dto.Emails);
            foreach (var email in emails)
            {
                email.EmpresaId = empresa.EmpresaId;
                email.CriadoEm = DateTime.UtcNow;
                email.CriadoPorId = usuario.Id;
            }

            // 8. Preparar telefones
            if (dto.Telefones == null)
                throw new ArgumentException("A empresa deve conter pelo menos um telefone de contato.");

            var telefones = _mapper.Map<List<EmpresaTelefone>>(dto.Telefones);
            foreach (var telefone in telefones)
            {
                //verifica se o número de telefone possui um valor válido antes de tentar adicionar só numeros 
                if(_phoneValidator.IsValid(telefone.Numero)) 
                   telefone.Numero = _phoneValidator.RemoveFormatting(telefone.Numero);

                telefone.EmpresaId = empresa.EmpresaId;
                telefone.CriadoEm = DateTime.UtcNow;
                telefone.CriadoPorId = usuario.Id;
            }

            // 9. Criar a empresa PRIMEIRO (antes de vincular o usuário)
            await _empresaRepository.CriarEmpresaAsync(empresa);

            _logger.LogInformation("Empresa {EmpresaId} criada com sucesso pelo usuário {UserId}",
                empresa.EmpresaId, usuario.Id);

            // 10. Adicionar emails e telefones que referenciam a empresa
            await _empresaRepository.AdicionarEmailsAsync(emails);
            await _empresaRepository.AdicionarTelefonesAsync(telefones);

            // 11. AGORA vincular usuário à empresa (empresa já existe no banco)
            usuario.EmpresaId = empresa.EmpresaId;

            // 12. Atribuir role "Empresa" se não tiver (isso salva o usuário automaticamente)
            if (!await _userManager.IsInRoleAsync(usuario, "Empresa"))
            {
                await _userManager.AddToRoleAsync(usuario, "Empresa");
                _logger.LogInformation("Role 'Empresa' atribuída ao usuário {UserId}", usuario.Id);
            }
            else
            {
                // Se já tem a role, precisa atualizar manualmente para salvar o EmpresaId
                var usuarioAtualizado = await _usuarioRepository.UpdateUserAsync(usuario);
                if (!usuarioAtualizado)
                {
                    _logger.LogError("Falha ao vincular usuário {UserId} à empresa {EmpresaId}", usuario.Id, empresa.EmpresaId);
                    throw new InvalidOperationException("Erro ao vincular usuário à empresa.");
                }
            }

            // 13. Buscar empresa completa e retornar DTO
            var empresaCompleta = await _empresaRepository.GetEmpresaCompletaAsync(empresa.EmpresaId);

            return await MapEmpresaToSimpleDTOAsync(empresaCompleta);
        }

        public async Task<IEnumerable<EmpresaSimpleDTO>> GetEmpresaByUserAsync()
        {
            var currentUser = await _usuarioRepository.GetCurrentUser();
            if (currentUser == null)
            {
                _logger.LogWarning("Tentativa de acessar empresa sem estar autenticado.");
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            var empresas = await _empresaRepository.GetAllEmpresaByUser(currentUser.Id);
            if (empresas == null)
            {
                _logger.LogInformation("Nenhuma empresa encontrada para o usuário {UserId}", currentUser.Id);
                throw new KeyNotFoundException("Nenhuma empresa associada ao usuário.");
            }

            var empresaDTO = new List<EmpresaSimpleDTO>();

            foreach (var empresa in empresas)
            {
                _logger.LogInformation("Empresa encontrada para o usuário {UserId}: {EmpresaId} - {RazaoSocial}",
                    currentUser.Id, empresa.EmpresaId, empresa.RazaoSocial ?? "N/A");
                
                var dto = await MapEmpresaToSimpleDTOAsync(empresa);
                empresaDTO.Add(dto);
            }

            return empresaDTO;
        }

        // serviço de edição de empresa
        public async Task<EmpresaSimpleDTO> UpdateEmpresaAsync(Guid empresaId, EmpresaEditDTO dto)
        {
            //1. Obter usuário autenticado
            var currentUser = await _usuarioRepository.GetCurrentUser();
            if (currentUser == null)
            {
                _logger.LogWarning("Tentativa de editar empresa sem estar autenticado.");
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }

            //2. Buscar empresa completa para verificar se existe e se o usuário é responsável
            var empresa = await _empresaRepository.GetEmpresaCompletaAsync(empresaId);
            if (empresa == null)
            {
                _logger.LogWarning("Empresa não encontrada para edição: {EmpresaId}", empresaId);
                throw new KeyNotFoundException("Empresa não encontrada.");
            }

            // Verificar se o usuário é responsável pela empresa
            if (empresa.UsuarioResponsavelId != currentUser.Id)
            {
                _logger.LogWarning("Usuário {UserId} tentou editar empresa {EmpresaId} sem ser responsável.",
                    currentUser.Id, empresaId);
                throw new UnauthorizedAccessException("Você não tem permissão para editar esta empresa.");
            }

            //3. Atualizar apenas os campos fornecidos (não-null)
            if (dto.RazaoSocial != null)
                empresa.RazaoSocial = dto.RazaoSocial;

            if (dto.NomeFantasia != null)
                empresa.NomeFantasia = dto.NomeFantasia;

            if (dto.CNPJ != null)
            {
                // Validar e limpar CNPJ (remover formatação)
                var cnpjLimpo = _docValidatorService.ValidarCNPJ(dto.CNPJ);
                if (cnpjLimpo == null)
                {
                    _logger.LogWarning("CNPJ inválido fornecido: {CNPJ} por usuário {UserId}", dto.CNPJ, currentUser.Id);
                    throw new ArgumentException("CNPJ inválido.");
                }

                // Verificar se o CNPJ mudou E se já existe
                if (empresa.CNPJ != cnpjLimpo)
                {
                    if (await _empresaRepository.CNPJExisteAsync(cnpjLimpo))
                    {
                        _logger.LogWarning("CNPJ já cadastrado para outra empresa: {CNPJ}", cnpjLimpo);
                        throw new ArgumentException("Este CNPJ já está cadastrado para outra empresa.");
                    }
                }

                empresa.CNPJ = cnpjLimpo;
            }

            if (dto.InscricaoEstadual != null)
                empresa.InscricaoEstadual = dto.InscricaoEstadual;

            if (dto.InscricaoMunicipal != null)
                empresa.InscricaoMunicipal = dto.InscricaoMunicipal;

            // Atualizar endereço se fornecido
            if (dto.CEP != null)
                empresa.CEP = dto.CEP;

            if (dto.Logradouro != null)
                empresa.Logradouro = dto.Logradouro;

            if (dto.Numero != null)
                empresa.Numero = dto.Numero;

            if (dto.Complemento != null)
                empresa.Complemento = dto.Complemento;

            if (dto.Bairro != null)
                empresa.Bairro = dto.Bairro;

            if (dto.Cidade != null)
                empresa.Cidade = dto.Cidade;

            if (dto.UF != null)
                empresa.UF = dto.UF;

            // Atualizar metadados de auditoria
            empresa.AtualizadoEm = DateTime.UtcNow;
            empresa.AtualizadoPorId = currentUser.Id;

            // Atualizar a empresa no banco
            var result = await _empresaRepository.UpdateEmpresaAsync(empresa);
            if (!result)
            {
                _logger.LogError("Falha ao atualizar empresa {EmpresaId} pelo usuário {UserId}.",
                    empresaId, currentUser.Id);
                throw new InvalidOperationException("Falha ao atualizar a empresa, tente novamente mais tarde!");
            }

            _logger.LogInformation("Empresa {EmpresaId} atualizada com sucesso pelo usuário {UserId}.",
                empresaId, currentUser.Id);

            // Retornar a empresa atualizada
            var empresaCompleta = await _empresaRepository.GetEmpresaCompletaAsync(empresa.EmpresaId);
            return await MapEmpresaToSimpleDTOAsync(empresaCompleta);
        }

        public async Task<bool> DeleteEmpresaAsync(Guid empresaId)
        {
            var currentUser = await _usuarioRepository.GetCurrentUser();
            var usuario = await _usuarioRepository.GetUserByIdAsync(currentUser.Id);

            if (currentUser == null)
            {
                _logger.LogWarning("Tentativa de deletar empresa sem estar autenticado.");
                throw new UnauthorizedAccessException("Usuário não autenticado.");
            }
            var empresa = await _empresaRepository.GetEmpresaCompletaAsync(empresaId);
            if (empresa == null)
            {
                _logger.LogWarning("Empresa não encontrada para exclusão: {EmpresaId}", empresaId);
                throw new KeyNotFoundException("Empresa não encontrada.");
            }
            if (empresa.UsuarioResponsavelId != currentUser.Id)
            {
                _logger.LogWarning("Usuário {UserId} tentou deletar empresa {EmpresaId} sem ser responsável.",
                    currentUser.Id, empresaId);
                throw new UnauthorizedAccessException("Você não tem permissão para deletar esta empresa.");
            }

            //desvincular usuário da empresa antes de deletar
            usuario.EmpresaId = null;

            //retirar a role de empresa do usuário
            if (await _userManager.IsInRoleAsync(usuario, "Empresa"))
            {
                await _userManager.RemoveFromRoleAsync(usuario, "Empresa");
                _logger.LogInformation("Role 'Empresa' removida do usuário {UserId} após exclusão da empresa.", usuario.Id);
            }

            var result = await _empresaRepository.DeleteEmpresaAsync(empresaId);
            if (result)
            {
                _logger.LogInformation("Empresa {EmpresaId} deletada com sucesso pelo usuário {UserId}.",
                    empresaId, currentUser.Email);
            }
            else
            {
                _logger.LogError("Falha ao deletar empresa {EmpresaId} pelo usuário {UserId}.",
                    empresaId, currentUser.Email);
                throw new ArgumentException("Falha ao deletar a empresa, tente novamente mais tarde!");
            }
            await _usuarioRepository.UpdateUserAsync(usuario);

            return result;
        }

        private async Task<EmpresaSimpleDTO> MapEmpresaToSimpleDTOAsync(Empresa empresa)      
        {
            var dto = _mapper.Map<EmpresaSimpleDTO>(empresa);
            dto.CNPJ = _formatService.SetupFormatDocument(dto.CNPJ);

            var telefones = await _empresaRepository.GetAllTelefonesByEmpresaIdAsync(dto.Id);
            dto.Telefones = _mapper.Map<List<EmpresaTelefoneDTO>>(telefones);

            var emails = await _empresaRepository.GetAllEmailsByEmpresaIdAsync(dto.Id);
            dto.Emails = _mapper.Map<List<EmpresaEmailDTO>>(emails);

            var usuariosVinculados = await _userService.GetAllUsuariosByEmpresaIdAsync(dto.Id);
            dto.UsuariosVinculados = _mapper.Map<List<UserSimpleDTO?>>(usuariosVinculados);

            foreach (var telefone in dto.Telefones)
            {
                telefone.Numero = _formatService.SetupFormatPhone(telefone.Numero);
            }

            return dto;
        }
    }
}
