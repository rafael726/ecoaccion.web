using ecoaccion.Core.DTOs.Interacciones;
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;

namespace ecoaccion.Core.Interfaces.Repositories
{
    public interface IinteraccionRepository:IBaseRepository<Interaccion>
    {
        Task AddInteraccionAsync(Interaccion interaccion);
        Task UpdateInteraccionAsync( UpdateInteraccionDto updateInteraccionDto, int id );
        void DeleteInteraccionAsync( int id );
        Task<IEnumerable<Interaccion>> GetAll();
        Task<Interaccion?> GetById( int id );
        public IEnumerable<Usuario> SearchUser( Func<Usuario, bool> filer );
        public IEnumerable<Administrador> SearchAdmin( Func<Administrador, bool> filter );
    }
}
