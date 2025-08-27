using ecoaccion.Core.DTOs.Participaciones;
using ecoaccion.Core.Entities;

namespace ecoaccion.Core.Interfaces.Repositories
{
    public interface IParticipacionRepository:IBaseRepository<Participacion>
    {
        Task<bool> UpdateParticipacionProgresoAsync( ParticipacionProgresoUpdateDto dto );
        Task<bool> CargarEvidenciaAsync(int idUsuario, int idDesafio, byte[] evidencia);
        IEnumerable<Participacion> GetHistorialParticipacion( int idUsuario );
        Task<IEnumerable<Participacion>> GetHistorialParticipacionAsync( int idUsuario );
        Task<bool> AddParticipacionAsync( ParticipicacionInsertDto participicacionInsertDto );
        IEnumerable<Usuario> Search( Func<Usuario, bool> filer );
    }
}
