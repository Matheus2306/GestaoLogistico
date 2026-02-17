using AutoMapper;
using GestaoLogistico.DTOs;
using GestaoLogistico.Models;
using System.Diagnostics;

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
            CreateMap<Usuario, UserSimpleDTO>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Nome, opt => opt.MapFrom(src => src.NomeCompleto))
                .ForMember(dest => dest.CPF, opt => opt.MapFrom(src => src.CPF))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.UrlPhoto, opt => opt.MapFrom(src => src.UrlFoto))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()); // Roles serão preenchidos manualmente
        }
    }
}
