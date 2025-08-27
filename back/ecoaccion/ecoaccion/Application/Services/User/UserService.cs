using ecoaccion.Application.Services.Auth;
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Core.Interfaces.Services.Auth;
using ecoaccion.Core.Interfaces.Services.User;
using ecoaccion.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ecoaccion.Application.Services.User
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtService _jwtService;
        private readonly IPasswordService _passwordService;

        public UserService( [FromKeyedServices("userRepository")] IUserRepository userRepository,
            JwtService jwtService, IPasswordService passwordService
            )
        {
            _userRepository = userRepository;
            _jwtService = jwtService;
            _passwordService = passwordService;
            Erros = new List<string>();
        }
        public List<string> Erros { get; }

        public async Task<UserDto> Add( UserInsertDto userInsertDto )
        {
            userInsertDto.Contraseña = _passwordService.HashPassword(userInsertDto.Contraseña);
            var user = new Usuario
            {
                NombreUsuario = userInsertDto.NombreUsuario,
                Correo = userInsertDto.Correo,
                Contrasena = userInsertDto.Contraseña
            };
            await _userRepository.AddAsync(user);
            await _userRepository.SaveChangesAsync();
            var userDto = new UserDto
            {
                NombreUsuario = user.NombreUsuario,
                Correo = user.Correo,
            };
            return userDto;
        }

        public bool Delete( int id )
        {
            _userRepository.DeleteUserAsync(id);
            _userRepository.SaveChangesAsync();
            return true;
        }
        
        public async Task<IEnumerable<Usuario>> GetAll() 
            => await _userRepository.GetAll();

        public async Task<UserDto> GetById( int id ) 
            => await _userRepository.GetById(id);

        public string Login( LoginUserDto loginUserDto )
        {
            var user = _userRepository.Search(u => u.Correo == loginUserDto.Correo).FirstOrDefault();
            var token = _jwtService.GenerateToken(user.IdUsuario.ToString(), user.Correo, "user");
            return token;
        }

        public async Task<UserUpdateDto?> Update( UserUpdateDto userUpdateDto, int id )
        {
            var user = await _userRepository.GetByIdAsync(id);
            userUpdateDto.Contraseña = _passwordService.HashPassword(userUpdateDto.Contraseña);
            user.Correo = userUpdateDto.Correo;
            user.Contrasena = userUpdateDto.Contraseña;
            await _userRepository.UpdateUserAsync(userUpdateDto, id);
            await _userRepository.SaveChangesAsync();

            return userUpdateDto;
        }

        public bool Validate( UserInsertDto userInsertDto )
        {
            if (_userRepository.Search(u => u.Correo == userInsertDto.Correo).Count() > 0)
            {
                Erros.Add("El correo ya se encuentra registrado");
                return false;
            }
            if (_userRepository.Search(u => u.NombreUsuario == userInsertDto.NombreUsuario).Count() > 0)
            {
                Erros.Add("El nombre de usuario ya se encuentra registrado");
                return false;
            }
            return true;
        }

        public bool Validate( int id )
        {
         if (_userRepository.Search(u => u.IdUsuario == id).Count() == 0)
            {
                Erros.Add("El usuario no existe");
                return false;
            }
            return true;
        }

        public bool Validate( LoginUserDto loginUserDto )
        {
            var user = _userRepository.Search(u => u.Correo == loginUserDto.Correo).FirstOrDefault();
            if (user == null)
            {
                Erros.Add("Email invalido");
                return false;
            }
            if(!_passwordService.VerifyPassword(loginUserDto.Contraseña, user.Contrasena))
            {
                Erros.Add("Contraseña incorrecta");
                return false;
            }
            return true;
        }

        public async Task<IEnumerable<UserClasificacionDto>> GetClasificacionUsuariosAsync()
        {
            var usuarios = await _userRepository.GetAll();
            var result = usuarios.Select(u => new UserClasificacionDto
            {
                IdUsuario = u.IdUsuario,
                NombreUsuario = u.NombreUsuario,
                Puntos = u.Puntos ?? 0,
                DesafiosCumplidos = (u.Participaciones ?? new List<Participacion>()).Count(p => p.Progreso == "completado")
            }).OrderByDescending(u => u.Puntos);
            return result;
        }
    }
}
