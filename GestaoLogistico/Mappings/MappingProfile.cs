using AutoMapper;
using GestaoLogistico.DTOs.UsersDTO;
using GestaoLogistico.Models;
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
        }
    }
}
