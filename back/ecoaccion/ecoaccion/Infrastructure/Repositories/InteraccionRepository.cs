#nullable disable
using ecoaccion.Core.DTOs.Interacciones;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Infrastructure.Common;
using ecoaccion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ecoaccion.Infrastructure.Repositories
{
    public class InteraccionRepository( AppDbContext context ) : BaseRepository<Interaccion>(context), IinteraccionRepository
    {
        public async Task AddInteraccionAsync( Interaccion interaccion ) 
            => await _context.Interacciones.AddAsync( interaccion );

        public void DeleteInteraccionAsync( int id )
        {
            var interaccion = _context.Interacciones.Where(i => i.IdInteraccion == id).FirstOrDefault();
            _context.Interacciones.Remove(interaccion);
        }

        public async Task<IEnumerable<Interaccion>> GetAll()        
            => await _context.Interacciones.ToListAsync();
        


        public Task<Interaccion> GetById( int id )
        {
            throw new NotImplementedException();
        }

        public async Task UpdateInteraccionAsync( UpdateInteraccionDto updateInteraccionDto, int id )
        {
            var interaccion = await _context.Interacciones.FirstOrDefaultAsync(i => i.IdInteraccion == id);
            _context.Interacciones.Update( interaccion );
        }
        public  IEnumerable<Usuario> SearchUser( Func<Usuario, bool> filer )
           => _context.Usuarios.Where(filer).ToList();

        public IEnumerable<Administrador> SearchAdmin(Func<Administrador, bool> filter ) 
            =>_context.Administradores.Where(filter).ToList();
    }
}
