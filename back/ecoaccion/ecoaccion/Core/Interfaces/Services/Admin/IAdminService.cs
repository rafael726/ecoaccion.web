using ecoaccion.Core.DTOs.Admin;
using ecoaccion.Core.DTOs.User;

namespace ecoaccion.Core.Interfaces.Services.Admin
{
    public interface IAdminService
    {
        public List<string> Erros { get; }
        Task<AdminDto> Add( AdminInsertDto adminInsertDto );
        Task<bool> Delete( object id );
        Task<AdminUpdateDto> Update( AdminUpdateDto adminInsertDto );
        Task<AdminDto> GetById( object id );
        Task<IEnumerable<AdminDto>> GetAll();
        bool Validate( AdminInsertDto adminInsertDto );
        bool Validate( LoginUserDto loginUserDto );
        string Login( LoginUserDto loginUserDto );
    }
}
