using ecoaccion.Core.DTOs.Admin;
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Services.Admin;
using ecoaccion.Core.Interfaces.Services.User;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecoaccion.Api.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly IValidator<AdminInsertDto> _adminValidator;
        public AdminController( 
            [FromKeyedServices("adminService")] IAdminService adminService,
            [FromKeyedServices("userService")]IUserService userService,
            IValidator<AdminInsertDto> adminValidator,
            IValidator<UserInsertDto> userInsertValidator
            )
        {
            _adminService = adminService;
            _adminValidator = adminValidator;
            
        }
        [HttpPost]
        public async Task<ActionResult<AdminDto>> Add( AdminInsertDto adminInsertDto )
        {
            var validationResult = await _adminValidator.ValidateAsync(adminInsertDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            if (!_adminService.Validate(adminInsertDto))
            {
                return BadRequest(_adminService.Erros);
            }
            var adminDto = await _adminService.Add(adminInsertDto);
            return Ok(adminDto);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(LoginUserDto loginUser )
        {
            if (!_adminService.Validate(loginUser))
            {
                return Unauthorized(_adminService.Erros);
            }
            var token = _adminService.Login(loginUser);
            return Ok(new {Token = token});
        }

    }
}
