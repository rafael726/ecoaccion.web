using ecoaccion.Core.DTOs.Admin;
using ecoaccion.Core.Entities;

namespace ecoaccion.Core.Interfaces.Repositories
{
    public interface IAdminRepository:IBaseRepository<Administrador>
    {
        Task AddAdminAsync( Administrador admin );
    }
}
