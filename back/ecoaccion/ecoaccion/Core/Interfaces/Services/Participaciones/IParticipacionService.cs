using ecoaccion.Core.DTOs.Participaciones;
using ecoaccion.Core.Entities;

namespace ecoaccion.Core.Interfaces.Services.Participaciones
{
    public interface IParticipacionService
    {
        public List<string> Erros { get; }
        Task<bool> MarcarProgresoAsync( ParticipacionProgresoUpdateDto dto );
        Task<IEnumerable<ParticipacionDto>> GetHistorialParticipacion( int idUsuario );
        Task<bool> CrearParticipacionAsync( ParticipicacionInsertDto dto );
        Task<Participacion> GetImage( int id );
        //public bool Validate( ParticipicacionInsertDto dto );
        public bool Validate( int id );
        string GetImageMimeType( byte[] imageData );
        bool Validate( ParticipicacionInsertDto dto );
    }
}
