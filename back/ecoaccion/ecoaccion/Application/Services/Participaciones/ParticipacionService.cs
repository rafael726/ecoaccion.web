using AutoMapper;
using ecoaccion.Core.DTOs.Participaciones;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Core.Interfaces.Services.Participaciones;
using ecoaccion.Infrastructure.Repositories;

namespace ecoaccion.Application.Services.Participaciones
{
    public class ParticipacionService:IParticipacionService
    {
        public ParticipacionService( 
            [FromKeyedServices("participacionRepository")] IParticipacionRepository participacionRepository,
            IMapper mapper
            )
        {
            _participacionRepository = participacionRepository;
            _mapper = mapper;
            Erros = new List<string>();
        }
        private readonly IMapper _mapper;

        private readonly IParticipacionRepository _participacionRepository;

        public List<string> Erros { get; }

        public async Task<bool> MarcarProgresoAsync( ParticipacionProgresoUpdateDto dto )
        {
            return await _participacionRepository.UpdateParticipacionProgresoAsync(dto);
        }
        public async Task<IEnumerable<ParticipacionDto>> GetHistorialParticipacion(int idUsuario)
        {
            var historial = await _participacionRepository.GetHistorialParticipacionAsync(idUsuario);

            // Si usas AutoMapper:
            return historial.Select(p => _mapper.Map<ParticipacionDto>(p));
        }
        
        public async Task<bool> CrearParticipacionAsync(ParticipicacionInsertDto dto)
        {
            var participacion = _mapper.Map<ParticipicacionInsertDto>(dto);
            return await _participacionRepository.AddParticipacionAsync(participacion);
        }

        public async Task<Participacion?> GetImage( int id )
        {
            var participacion = _participacionRepository.Search(p => p.IdPart == id).FirstOrDefault();
            if (participacion == null || participacion.Evidencia == null)
            {
                return null;
            }
            return _mapper.Map<Participacion>(participacion);
        }

        public bool Validate( int id )
        {
            if(_participacionRepository.Search(p => p.IdPart == id).Count() == 0)
            {
                Erros.Add("No se econtró la participacion");
                return false;
            }
            return true;
        }

        public string GetImageMimeType( byte[] imageData )
        {
            if (imageData.Length >= 4)
            {
                if (imageData[0] == 0xFF && imageData[1] == 0xD8)
                    return "image/jpeg";

                if (imageData[0] == 0x89 && imageData[1] == 0x50 &&
                    imageData[2] == 0x4E && imageData[3] == 0x47)
                    return "image/png";

                if (imageData[0] == 0x47 && imageData[1] == 0x49 &&
                    imageData[2] == 0x46)
                    return "image/gif";
            }
            return "application/octet-stream";
        }

        public bool Validate( ParticipicacionInsertDto dto )
        {
            if (_participacionRepository.Search(p => p.IdDesafio == dto.IdDesafio).Count() > 0) 
            {
                Erros.Add("ya estás participando en este desafio");
                return false;
            }
            return true;
        }
    }
}
