using ecoaccion.Core.DTOs.Interacciones;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Services.Interacciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecoaccion.Api.Controllers.Interacciones
{
    [Route("api/[controller]")]
    [ApiController]
    public class InteraccionController : ControllerBase
    {
        private readonly IinteraccionService _interaccionService;
        public InteraccionController( [FromKeyedServices("interaccionService")]IinteraccionService iinteraccionService)
        {
            _interaccionService = iinteraccionService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<InteraccionDto>> Add(CreateinteraccionDto createinteraccionDto )
        {
            if (!_interaccionService.Validate(createinteraccionDto))
            {
                return BadRequest(_interaccionService.Errors);
            }
            var interaccionDto = await _interaccionService.AddinteraccionAsync(createinteraccionDto);
            return Ok(interaccionDto);
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<InteraccionDto>> Get()
            => await _interaccionService.GetAll();

        [HttpDelete("{id}")]
        [Authorize]
        public IActionResult Delete( int id ) 
        {
            if (!_interaccionService.Validate(id))
            {
                return NotFound(_interaccionService.Errors);
            }
            return Ok(_interaccionService.Delete(id));
        }
    }
}
