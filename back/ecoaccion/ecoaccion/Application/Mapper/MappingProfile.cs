using AutoMapper;
using ecoaccion.Application.Mapper.ValueResolver;
using ecoaccion.Core.DTOs.Desafio;
using ecoaccion.Core.DTOs.Participaciones;
using ecoaccion.Core.Entities;

namespace ecoaccion.Application.Mapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Desafio, DesafioDto>()
                .ForMember(dest => dest.ImagenComprob, opt => opt.MapFrom<ImageValueResolver>());

            CreateMap<InsertDesafioImageDto, Desafio>()
                .ForMember(des => des.ImagenComprob, opt => opt.MapFrom(src => ConvertToByteArray(src.ImagenComprob)))
                .ForMember(dest => dest.IdAdmin, opt => opt.Ignore())
                .ForMember(dest => dest.IdDesafio, opt => opt.Ignore());

            CreateMap<Participacion, ParticipacionDto>()
                .ForMember(des => des.Evidencia, opt => opt.MapFrom<ImageEvidenciaValueResolver>())
                .ForMember(dest => dest.TituloDesafio, opt => opt.MapFrom(src => src.Desafio != null ? src.Desafio.Titulo : null));

            CreateMap<ParticipicacionInsertDto, Participacion>()
                .ForMember(dest => dest.Evidencia, opt => opt.MapFrom(src => ConvertToByteArray(src.Evidencia)))
                .ForMember(dest => dest.FechaRegistro, opt => opt.MapFrom(_ => DateTime.UtcNow));
        }
        private static byte[]? ConvertToByteArray( IFormFile? file )
        {
            if (file == null) return null;
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
