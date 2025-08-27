using ecoaccion.Core.DTOs.Desafio;
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace ecoaccion.Core.Interfaces.Repositories
{
    public interface IDesafioRepository:IBaseRepository<Desafio>
    {
        Task AddDesafioAsync( Desafio desafio );
        void DeleteDesafioAsync( int id );
        Task<IEnumerable<Desafio>> GetAll();
        Task<Desafio?> GetById( int id );
        Task AddImage(InsertDesafioImageDto image);
        IEnumerable<Desafio> SearchByTituloOrMeta( string search );
        IEnumerable<Participacion> GetParticipacionesByDesafio( int idDesafio );
        Task UpdateAsync( DesafioUpdateDto dto, int id );


    }
}
