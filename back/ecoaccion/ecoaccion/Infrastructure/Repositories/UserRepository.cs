#nullable disable
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Infrastructure.Common;
using ecoaccion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ecoaccion.Infrastructure.Repositories
{
    public class UserRepository( AppDbContext context ) : BaseRepository<Usuario>(context), IUserRepository
    {
        public async Task AddUserAsync( Usuario user ) => await _context.Usuarios.AddAsync(user);

        public void DeleteUserAsync( int id )
        {
            var user = _context.Usuarios.Where(u => u.IdUsuario == id).FirstOrDefault();
               _context.Usuarios.Remove(user);
        
            
        }

        public async Task<IEnumerable<Usuario>> GetAll()     
            => await _context.Usuarios.Include(p => p.Participaciones).ToListAsync();
    
       
        

        public async Task UpdateUserAsync( UserUpdateDto userUpdateDto , int id )
        {
            var user = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
            _context.Usuarios.Update(user);
        }

        public async Task<UserDto> GetById(int id)
        {
            var user = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (user == null) return null;

            return new UserDto
            {
                IdUsuario = user.IdUsuario,
                NombreUsuario = user.NombreUsuario,
                Correo = user.Correo,
                Puntos = user.Puntos ?? 0
                // Agrega solo los campos simples que necesitas
            };
        }
    }
}
