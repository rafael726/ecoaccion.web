using AutoMapper;
using ecoaccion.Core.DTOs.Desafio;
using ecoaccion.Core.Entities;

namespace ecoaccion.Application.Mapper.ValueResolver
{
    public class ImageValueResolver:IValueResolver<Desafio, DesafioDto, string?>
    {
        private readonly IHttpContextAccessor _contextAccessor;
        public ImageValueResolver(IHttpContextAccessor contextAccessor) 
        {
            _contextAccessor = contextAccessor;
        }
        public string? Resolve(Desafio source, DesafioDto destination, string? destMember, ResolutionContext context )
        {
            if (source.ImagenComprob == null) return null;
            var request = _contextAccessor.HttpContext?.Request;
            if (request == null) return null;
            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}/api/desafio/{source.IdDesafio}/image";
        }
    }
}
