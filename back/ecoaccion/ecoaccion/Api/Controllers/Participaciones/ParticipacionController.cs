using ecoaccion.Core.DTOs.Participaciones;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Services.Participaciones;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecoaccion.Api.Controllers.Participaciones
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParticipacionController : ControllerBase
        
    {
        private readonly IParticipacionService _participacionService;

        public ParticipacionController( [FromKeyedServices("participacionService")] IParticipacionService participacionService)
        {
            _participacionService = participacionService;
        }

        /// <summary>
        /// Actualiza la participacion, el valor del campo progreso deberá ser igual a "completado" si se quiere marcar como finalizada 
        /// </summary>
        /// <returns>Retorna status code 200 .</returns>
        [Authorize]
        [HttpPut("marcar-progreso")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> MarcarProgreso( [FromForm] ParticipacionProgresoUpdateDto dto )
        {
            var result = await _participacionService.MarcarProgresoAsync(dto);
            if (!result)
                return NotFound("Participación no encontrada.");
            return Ok("Progreso actualizado correctamente.");
        }
        [Authorize]
        [HttpGet("historial/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<ParticipacionDto>>> GetHistorialParticipacion(int idUsuario)
        {
            var historial = await _participacionService.GetHistorialParticipacion(idUsuario);
            return Ok(historial);
        }

        /// <summary>
        /// El usuario puede crear una nueva evidencia de participacion, el progeso se puede iniciar en cuanquier valor string diferente de completado
        /// </summary>
        /// <returns>Retorna status code 200 .</returns>
        [Authorize]
        [HttpPost("crear")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CrearParticipacion([FromForm] ParticipicacionInsertDto dto)
        {
            if (!_participacionService.Validate(dto))
            {
                return BadRequest(_participacionService.Erros);
            }
            await _participacionService.CrearParticipacionAsync(dto);
            return Ok("Participación creada correctamente.");
        }

        [HttpGet("{id}/image")]
        [EnableCors("ImagesPolicy")]
        public async Task<IActionResult> GetImage(int id )
        {
            if (!_participacionService.Validate(id))
            {
                return BadRequest(_participacionService.Erros);
            }
            var desafio = await _participacionService.GetImage(id);
            if (desafio?.Evidencia == null || desafio.Evidencia.Length == 0)
            {
                return NotFound("Imagen no encontrada.");
            }
            var mimeType = _participacionService.GetImageMimeType(desafio.Evidencia);
            return File(desafio.Evidencia, mimeType);

        }
    }
}
