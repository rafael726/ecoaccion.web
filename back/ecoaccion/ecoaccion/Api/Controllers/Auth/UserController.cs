using ecoaccion.Application.Validator.Auth;
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Services.User;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecoaccion.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IValidator<UserInsertDto> _userInsertValidator;
        private readonly IValidator<UserUpdateDto> _userUpdateValidator;

        public UserController( 
            [FromKeyedServices("userService")] IUserService userService, 
            IValidator<UserInsertDto> userInsertValidator, 
            IValidator<UserUpdateDto> userUpdateValidator
            )
        {
            _userInsertValidator = userInsertValidator;
            _userUpdateValidator = userUpdateValidator;
            _userService = userService;
        }
        [HttpPost]
        public async Task<ActionResult<UserDto>> Add( UserInsertDto userInsertDto )
        {
            var validationResult = await _userInsertValidator.ValidateAsync(userInsertDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            if (!_userService.Validate(userInsertDto))
            {
                return BadRequest(_userService.Erros);
            }
            var userDto = await _userService.Add(userInsertDto);
            return Ok(userDto);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetById( int id )
        {
            if (!_userService.Validate(id))
            {
                return BadRequest(_userService.Erros);
            }
            var user = await _userService.GetById(id);
            return user;
        }

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete( int id )
        {
            if (!_userService.Validate(id))
            {

                return BadRequest(_userService.Erros);
            }
            return Ok(_userService.Delete(id));
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<UserUpdateDto>> Update(int id, UserUpdateDto userUpdateDto)
        {
            var validationResult = await _userUpdateValidator.ValidateAsync(userUpdateDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            if (!_userService.Validate(id))
            {

                return BadRequest(_userService.Erros);
            }
            var user = await _userService.Update(userUpdateDto, id);
            return user;
        }

        [HttpPost("login")]
        public  ActionResult<string> Login(LoginUserDto loginUserDto)
        {
            if (!_userService.Validate(loginUserDto))
            {
                return Unauthorized(_userService.Erros);
            }
            var token = _userService.Login(loginUserDto);
            return Ok(new { Token = token });
        }
        /// <summary>
        /// Obtiene la clasificación de todos los usuarios.
        /// </summary>
        /// <returns>Lista de usuarios con sus puntos y desafíos cumplidos.</returns>
        [HttpGet("clasificacion")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserClasificacionDto>>> GetClasificacion()
        {
            var clasificacion = await _userService.GetClasificacionUsuariosAsync();
            return Ok(clasificacion);
        }
    }
}
