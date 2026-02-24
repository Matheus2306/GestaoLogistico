using AutoMapper;
using GestaoLogistico.DTOs.UsersDTO;
using GestaoLogistico.Models;
using GestaoLogistico.Models.Empresas;
using System.Diagnostics;
using System.Net;

namespace GestaoLogistico.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Usuario, UserDTOcompleto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeCompleto))
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF))
                .ForMember(dest => dest.atualizadoEm, opt => opt.MapFrom(src => src.AtualizadoEm.HasValue ? src.AtualizadoEm.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty))
                .ForMember(dest => dest.criadoPor, opt => opt.MapFrom(src => src.CriadoPorId))
                .ForMember(dest => dest.atualizadoPor, opt => opt.MapFrom(src => src.AtualizadoPorId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.UrlFoto))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles serão preenchidos manualmente
            // O mapeamento acima é responsável por mapear as propriedades do modelo de usuário (Usuario) para o DTO completo (UserDTOcompleto), incluindo a formatação da data de atualização e a exclusão do mapeamento das roles, que serão preenchidas manualmente posteriormente.
            CreateMap<Usuario, UserSimpleDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeCompleto))
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UrlPhoto, opt => opt.MapFrom(src => src.UrlFoto))
                .ForMember(dest => dest.Roles, opt => opt.Ignore());// Roles serão preenchidos manualmente

            // O mapeamento acima é responsável por mapear as propriedades do modelo de usuário (Usuario) para o DTO simples (UserSimpleDTO), incluindo a formatação da data de atualização e a exclusão do mapeamento das roles, que serão preenchidas manualmente posteriormente. .ForAllOtherMembers(opt => opt.Ignore()); // Ignora outras propriedades que não estão mapeadas

            CreateMap<UserEditCreateDTO, Usuario>()
                .ForMember(dest => dest.NomeCompleto, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UrlFoto, opt => opt.Ignore());

            CreateMap<Usuario, UserEditCreateDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeCompleto))
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UrlPhoto, opt => opt.Ignore());

            CreateMap<Usuario, UserEditFormDTO>()
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeCompleto))
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.PhotoFile, opt => opt.Ignore());
            
            CreateMap<UserEditFormDTO, Usuario>()
                .ForMember(dest => dest.NomeCompleto, opt => opt.MapFrom(src => src.Nome))
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UrlFoto, opt => opt.Ignore());

            // Mappings de Empresa
            CreateMap<DTOs.EmpresaDTO.CriarEmpresaDTO, Empresa>()
                .ForMember(dest => dest.EmpresaId, opt => opt.Ignore())
                .ForMember(dest => dest.Ativo, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioResponsavelId, opt => opt.Ignore())
                .ForMember(dest => dest.UsuarioResponsavel, opt => opt.Ignore())
                .ForMember(dest => dest.Emails, opt => opt.Ignore())
                .ForMember(dest => dest.Telefones, opt => opt.Ignore())
                .ForMember(dest => dest.Usuarios, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoEm, opt => opt.Ignore())
                .ForMember(dest => dest.CriadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.AtualizadoPorId, opt => opt.Ignore())
                .ForMember(dest => dest.Excluido, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoEm, opt => opt.Ignore())
                .ForMember(dest => dest.ExcluidoPorId, opt => opt.Ignore());

            CreateMap<Empresa, DTOs.EmpresaDTO.EmpresaDTOCompleto>()
                .ForMember(dest => dest.Emails, opt => opt.MapFrom(src => src.Emails))
                .ForMember(dest => dest.Telefones, opt => opt.MapFrom(src => src.Telefones))
                .ForMember(dest => dest.UsuarioResponsavelNome, opt => opt.MapFrom(src => src.UsuarioResponsavel != null ? src.UsuarioResponsavel.NomeCompleto : null));

            CreateMap<EmpresaEmail, DTOs.EmpresaDTO.EmpresaEmailDTO>();
            CreateMap<EmpresaTelefone, DTOs.EmpresaDTO.EmpresaTelefoneDTO>();
        }
    }
}
