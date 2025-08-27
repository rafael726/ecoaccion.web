using ecoaccion.Application.Services.Auth;
using ecoaccion.Core.DTOs.Admin;
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Core.Interfaces.Services.Admin;
using ecoaccion.Core.Interfaces.Services.Auth;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ecoaccion.Application.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;
        private readonly JwtService _jwtService;
        private readonly IPasswordService _passwordService;
        public List<string> Erros { get; }

        public AdminService( 
            [FromKeyedServices("adminRepository")] IAdminRepository adminRepository,
            JwtService jwtService, IPasswordService passwordService
            )
        {
            _adminRepository = adminRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
            Erros = new List<string>();
        }
        public async Task<AdminDto> Add( AdminInsertDto adminInsertDto )
        {
            adminInsertDto.Contraseña = _passwordService.HashPassword( adminInsertDto.Contraseña );
            var admin = new Administrador
            {
                NombreUsuario = adminInsertDto.NombreUsuario,
                Correo = adminInsertDto.Correo,
                Contrasena = adminInsertDto.Contraseña,
            };
            await _adminRepository.AddAsync(admin);
            await _adminRepository.SaveChangesAsync();
            var adminDto = new AdminDto
            {
                NombreUsuario = admin.NombreUsuario,
                Correo = admin.Correo,
            };
            return adminDto;
            
        }

        public Task<bool> Delete( object id )
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<AdminDto>> GetAll<T>()
        {
            throw new NotImplementedException();
        }

        public Task<AdminDto> GetById( object id )
        {
            throw new NotImplementedException();
        }

        public Task<AdminUpdateDto> Update( AdminUpdateDto adminInsertDto )
        {
            throw new NotImplementedException();
        }

        public bool Validate( AdminInsertDto adminInsertDto )
        {
            if (_adminRepository.Search(a => a.Correo == adminInsertDto.Correo).Count() > 0)
            {
                Erros.Add("El correo ya se encuentra registrado");
                return false;
            }
            if (_adminRepository.Search(a => a.NombreUsuario == adminInsertDto.NombreUsuario).Count() > 0)
            {
                Erros.Add("El nombre de usuario ya se encuentra registrado");
                return false;
            }
            return true;
        }

        public string Login( LoginUserDto loginUserDto )
        {
            var user = _adminRepository.Search(a => a.Correo ==  loginUserDto.Correo).FirstOrDefault();
            var token = _jwtService.GenerateToken(user.IdAdmin.ToString(), user.Correo, "admin");
            return token;
        }

        public bool Validate( LoginUserDto loginUserDto )
        {
            var user = _adminRepository.Search(a => a.Correo == loginUserDto.Correo).FirstOrDefault();
            if (user == null)
            {
                Erros.Add("Email invalido");
                return false;
            }
            if (!_passwordService.VerifyPassword(loginUserDto.Contraseña, user.Contrasena))
            {
                Erros.Add("Contraseña incorrecta");
                return false;
            }
            return true;
        }
    }
}
