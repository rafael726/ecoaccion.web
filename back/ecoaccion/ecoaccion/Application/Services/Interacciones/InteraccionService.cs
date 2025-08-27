using ecoaccion.Core.DTOs.Interacciones;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Core.Interfaces.Services.Interacciones;

namespace ecoaccion.Application.Services.Interacciones
{
    public class InteraccionService : IinteraccionService
    {
        private readonly IinteraccionRepository _interaccionRepository;
        public List<string> Errors { get; }
        public InteraccionService( [FromKeyedServices("interaccionRepository")]IinteraccionRepository interaccionRepository)
        {
            Errors = new List<string>();
            _interaccionRepository = interaccionRepository;
        }

        public async Task<InteraccionDto> AddinteraccionAsync( CreateinteraccionDto dto )
        {
            var colombiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var colombiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, colombiaTimeZone);

            var interaccion = new Interaccion
            {
                IdAdmin = dto.IdAdmin,
                IdUsuario = dto.IdUsuario,
                Tiempo = colombiaTime.TimeOfDay, // <-- Hora local de Colombia
                Fecha = DateTime.UtcNow,         // <-- Fecha en UTC (recomendado para DB)
                Tipo = dto.Tipo,
                Mensaje = dto.Mensaje,
            };
            await _interaccionRepository.AddInteraccionAsync( interaccion );
            await _interaccionRepository.SaveChangesAsync();
            return new InteraccionDto
            {
                IdAdmin = interaccion.IdAdmin,
                IdUsuario = interaccion.IdUsuario,
                Tiempo = interaccion.Tiempo,
                Fecha = interaccion.Fecha,
                Tipo= interaccion.Tipo,
                Mensaje= interaccion.Mensaje,
            };
        }

        public bool Delete( int id )
        {
            _interaccionRepository.DeleteInteraccionAsync(id);
            _interaccionRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<InteraccionDto>> GetAll()
        {
            var interacciones = await _interaccionRepository.GetAll();
            return interacciones.Select(i => new InteraccionDto
            {
                Fecha = i.Fecha,
                Tiempo = i.Tiempo,
                Tipo = i.Tipo,
                IdAdmin = i.IdAdmin,
                IdUsuario= i.IdUsuario,
                Mensaje = i.Mensaje,
            } );
        }

        public Task<Interaccion> GetById( int id )
        {
            throw new NotImplementedException();
        }

        public async Task<UpdateInteraccionDto> Update( UpdateInteraccionDto updateInteraccionDto, int id )
        {
            var interaccion = await _interaccionRepository.GetByIdAsync( id );
            interaccion.Mensaje = updateInteraccionDto.Mensaje;
            return updateInteraccionDto;
        }

        public bool Validate( CreateinteraccionDto createinteraccionDto )
        {
            if(_interaccionRepository.SearchUser(u => u.IdUsuario == createinteraccionDto.IdUsuario).Count() == 0)
            {
                Errors.Add("El usuario no existe");
                return false;
            }
            return true;
        }

        public bool Validate( UpdateInteraccionDto updateInteraccionDto )
        {
            {
                if (_interaccionRepository
                .Search(u => u.IdUsuario == updateInteraccionDto.IdUsuario
                || u.IdAdmin == updateInteraccionDto.IdAdmin).Count() == 0)
                {
                    Errors.Add("Debe ser el autor del mensaje");
                    return false;
                }
                return true;
            }
        }
        public bool Validate( int id ) 
        {
            if(_interaccionRepository.Search(i => i.IdInteraccion == id).Count() == 0)
            {
                Errors.Add("La interaccion no existe");
                return false;
            }
            return true;
        }
    }
}
