using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;

namespace ecoaccion.Core.Interfaces.Repositories
{
    public interface IUserRepository:IBaseRepository<Usuario>
    {
        Task AddUserAsync( Usuario user );
        Task UpdateUserAsync( UserUpdateDto userUpdateDto, int id);
        void DeleteUserAsync( int id );
        Task<IEnumerable<Usuario>> GetAll();
        Task<UserDto> GetById( int id );
    }
}
