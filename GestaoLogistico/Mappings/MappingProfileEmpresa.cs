using AutoMapper;
using GestaoLogistico.DTOs.EmpresaDTO;
using GestaoLogistico.Models.Empresas;

namespace GestaoLogistico.Mappings
{
    public class MappingProfileEmpresa : Profile
    {
        public MappingProfileEmpresa()
        {
            // mapeamento de empresa para empresasimpleDTO
            CreateMap<Empresa, EmpresaSimpleDTO>()
                .ForMember(dest => dest.FullAdress, opt => opt.MapFrom(src => 
                    string.Join(", ", new[] 
                    { 
                        $"{src.Logradouro} {src.Numero}",
                        src.Complemento,
                        src.Bairro,
                        $"{src.Cidade} - {src.UF}",
                        $"CEP: {src.CEP}"
                    }.Where(s => !string.IsNullOrWhiteSpace(s)))))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.EmpresaId))
                .ForMember(dest => dest.Emails, opt => opt.Ignore())
                .ForMember(dest => dest.Telefones, opt =>opt.Ignore())
                .ForMember(dest => dest.UsuariosVinculados, opt => opt.Ignore())
                .ForMember(dest => dest.UsuariosVinculados, opt => opt.Ignore());

            //Mappings de usuário responsável

            // Mappings de Empresa
            CreateMap<CriarEmpresaDTO, Empresa>()
                .ForMember(dest => dest.EmpresaId, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.Ativo, opt => opt.MapFrom(src => true))
                .ForMember(dest => dest.Emails, opt => opt.Ignore())
                .ForMember(dest => dest.Telefones, opt => opt.Ignore())
                .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioResponsavelId, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioResponsavel, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.Excluido, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoEm, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoPorId, opt => opt.Ignore());
                
            CreateMap<EmpresaEditDTO, Empresa>()
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())
                .ForMember(dest => dest.Emails, opt => opt.Ignore())
                .ForMember(dest => dest.Telefones, opt => opt.Ignore())
                .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioResponsavelId, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioResponsavel, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.Excluido, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoEm, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoPorId, opt => opt.Ignore());

            CreateMap<Empresa, EmpresaDTOCompleto>()
               .ForMember(dest => dest.Emails, opt => opt.MapFrom(src => src.Emails))
               .ForMember(dest => dest.Telefones, opt => opt.MapFrom(src => src.Telefones))
               .ForMember(dest => dest.UsuarioResponsavelNome, opt => opt.MapFrom(src => src.UsuarioResponsavel != null ? src.UsuarioResponsavel.NomeCompleto : null));

            CreateMap<EmpresaEmail, EmpresaEmailDTO>();
            CreateMap<EmpresaTelefone, EmpresaTelefoneDTO>();

            // Mappings reversos para criação
            CreateMap<EmpresaEmailDTO, EmpresaEmail>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Empresa, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.Excluido, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoEm, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoPorId, opt => opt.Ignore());

            CreateMap<EmpresaTelefoneDTO, EmpresaTelefone>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Empresa, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.Excluido, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoEm, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoPorId, opt => opt.Ignore());
        }
    }
}
