using ecoaccion.Core.DTOs.Admin;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Infrastructure.Common;
using ecoaccion.Infrastructure.Persistence;

namespace ecoaccion.Infrastructure.Repositories
{
    public class AdminRepository( AppDbContext context ) : BaseRepository<Administrador>(context), IAdminRepository
    {
        public async Task AddAdminAsync( Administrador admin ) => await _context.Administradores.AddAsync( admin );
        
    }
}
