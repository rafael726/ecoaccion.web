using ecoaccion.Core.DTOs.Desafio;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Services.Desafios;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ecoaccion.Api.Controllers.Desafios
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesafioController : ControllerBase
    {
        private readonly IDesafioService _desafioService;
        private readonly IValidator<DesafioInsertDto> _desafioInsertValidator;

        public DesafioController(
            [FromKeyedServices("desafioService")] IDesafioService desafioService,
            IValidator<DesafioInsertDto> validator )
        {
            _desafioService = desafioService;
            _desafioInsertValidator = validator;
        }

        /// <summary>
        /// Ceacion de desafios, permitido solo para usuarios con rol de admin.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<ActionResult<DesafioDto>> Add( DesafioInsertDto desafioInsertDto )
        {
            var validationResult = await _desafioInsertValidator.ValidateAsync(desafioInsertDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.Errors);
            }
            var desafioDto = await _desafioService.Add(desafioInsertDto);
            return Ok(desafioDto);
        }
        [Authorize(Roles = "admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete( int id )
        {
            if (!_desafioService.Validate(id))
            {
                return BadRequest(_desafioService.Erros);
            }
            return Ok(_desafioService.Delete(id));
        }

        [HttpGet]
        [Authorize]
        public async Task<IEnumerable<DesafioDto>> Get() => await _desafioService.GetAll();

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<DesafioDto>> GetById( int id )
        {
            if (!_desafioService.Validate(id))
            {
                return NotFound(_desafioService.Erros);
            }
            var desafio = await _desafioService.GetById(id);
            return desafio;
        }
        /// <summary>
        ///  Carga una imagen de comprobacion para el desafio.
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "admin")]
        [HttpPost("upload-image")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadImage( [FromForm] InsertDesafioImageDto imageDto )
        {
            if (!_desafioService.Validate(imageDto))
            {
                return BadRequest(_desafioService.Erros);
            }
            var desafioDto = await _desafioService.AddImage(imageDto);
            return Ok(desafioDto);
        }

        [HttpGet("{id}/image")]
        [EnableCors("ImagesPolicy")]
        public async Task<IActionResult> GetImage( int id )
        {
            if (!_desafioService.Validate(id))
            {
                return BadRequest(_desafioService.Erros);
            }
            var desafio = await _desafioService.GetImage(id);
            if (desafio?.ImagenComprob == null || desafio.ImagenComprob.Length == 0)
            {
                return NotFound("Imagen no encontrada.");
            }
            var mimeType = _desafioService.GetImageMimeType(desafio.ImagenComprob);
            return File(desafio.ImagenComprob, mimeType);

        }
        /// <summary>
        ///  Busqueda de desafios, la query titulo o meta.
        /// </summary>
        /// <returns></returns>
        [HttpGet("search")]
        [Authorize]
        public ActionResult<IEnumerable<DesafioDto>> Search([FromQuery] string q)
        {
            var resultados = _desafioService.SearchByTituloOrMeta(q);
            return Ok(resultados);
        }

        [HttpGet("{id}/progreso")]
        [Authorize]
        public ActionResult<DesafioProgresoDto> GetProgreso(int id)
        {
            var progreso = _desafioService.GetProgreso(id);
            if (progreso == null)
                return NotFound("Desafío no encontrado.");
            return Ok(progreso);
        }
        [Authorize(Roles = "admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update (DesafioUpdateDto dto, int id )
        {
            if (!_desafioService.Validate(id))
            {
                return BadRequest(_desafioService.Erros);
            }
            await _desafioService.Update(dto, id);
            return Ok("Desafio actualizado");
        }
    }
}
