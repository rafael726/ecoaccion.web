

using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;

namespace ecoaccion.Core.Interfaces.Services.User
{
    public interface IUserService
    {
        public List<string> Erros { get; }
        Task<UserDto> Add( UserInsertDto userInsertDto );
        bool Delete( int id );
        Task<UserUpdateDto> Update( UserUpdateDto userUpdateDto, int id );
        Task<UserDto> GetById( int id );
        Task<IEnumerable<UserDto>> GetAll();
        bool Validate( UserInsertDto userInsertDto );
        bool Validate( int id );
        bool Validate(LoginUserDto loginUserDto );
        string Login( LoginUserDto loginUserDto );
        Task<IEnumerable<UserClasificacionDto>> GetClasificacionUsuariosAsync();
    }
}
