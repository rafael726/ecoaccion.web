using ecoaccion.Core.DTOs.Interacciones;
using ecoaccion.Core.Entities;

namespace ecoaccion.Core.Interfaces.Services.Interacciones
{
    public interface IinteraccionService
    {
        public List<string> Errors { get; }
        Task<InteraccionDto> AddinteraccionAsync(CreateinteraccionDto dto);
        bool Delete( int id );
        Task<UpdateInteraccionDto> Update( UpdateInteraccionDto updateInteraccionDto, int id );
        Task<Interaccion> GetById( int id );
        Task<IEnumerable<InteraccionDto>> GetAll();
        bool Validate(CreateinteraccionDto createinteraccionDto);
        bool Validate( UpdateInteraccionDto updateInteraccionDto );
        bool Validate(int id);

    }
}
