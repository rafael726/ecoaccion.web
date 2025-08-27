using AutoMapper;
using ecoaccion.Core.DTOs.Participaciones;
using ecoaccion.Core.Entities;

namespace ecoaccion.Application.Mapper.ValueResolver
{
    public class ImageEvidenciaValueResolver:IValueResolver<Participacion, ParticipacionDto, string?>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public ImageEvidenciaValueResolver(IHttpContextAccessor contextAccessor )
        {
            _contextAccessor = contextAccessor;
        }
        public string? Resolve(Participacion source, ParticipacionDto destination, string? destMember, ResolutionContext context )
        {
            if(source.Evidencia == null) return null;
            var request = _contextAccessor.HttpContext?.Request;
            if(request == null) return null;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/api/participacion/{source.IdPart}/image";
        }
    }
}
