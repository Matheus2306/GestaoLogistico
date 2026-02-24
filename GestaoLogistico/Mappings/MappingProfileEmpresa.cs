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
                .ForMember(dest => dest.Emails, opt => opt.MapFrom(src => src.Emails))
                .ForMember(dest => dest.Telefones, opt =>opt.MapFrom(src => src.Telefones));
        }
    }
}
