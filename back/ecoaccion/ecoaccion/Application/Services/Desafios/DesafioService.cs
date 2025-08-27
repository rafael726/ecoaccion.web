using AutoMapper;
using ecoaccion.Core.DTOs.Desafio;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Core.Interfaces.Services.Desafios;

namespace ecoaccion.Application.Services.Desafios
{
    public class DesafioService : IDesafioService
    {
        private readonly IDesafioRepository _desafioRepository;
        private readonly IMapper _mapper;
        public List<string> Erros { get; }
        public DesafioService( [FromKeyedServices("desafioRepository")] IDesafioRepository desafioRepository, IMapper mapper ) 
        {
            _desafioRepository = desafioRepository;
            Erros = new List<string>();
            _mapper = mapper;
        }

        public async Task<DesafioDto> Add( DesafioInsertDto desafioInsertDto )
        {
            var desafio = new Desafio
            {
                Titulo = desafioInsertDto.Titulo,
                Descripcion = desafioInsertDto.Descripcion,
                Meta = desafioInsertDto.Meta,
                IdAdmin = desafioInsertDto.IdAdmin,
                FechaInicio = desafioInsertDto.FechaInicio,
                FechaFin = desafioInsertDto.FechaFin,
              
            };
            await _desafioRepository.AddAsync( desafio );
            await _desafioRepository.SaveChangesAsync();
            var desafioDto = new DesafioDto
            {
                Descripcion = desafio.Descripcion,
                Titulo = desafio.Titulo,
                Meta = desafio.Meta,
                IdAdmin = desafio.IdAdmin,
            };
            return desafioDto;
        }

        public bool Delete( int id )
        {
            _desafioRepository.DeleteDesafioAsync(id);
            _desafioRepository.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<DesafioDto>> GetAll()
        {
            var desafios = await _desafioRepository.GetAll(); 
            return desafios.Select(d => _mapper.Map<DesafioDto>(d));
        }

        public async Task<DesafioDto> GetById( int id )
        {
            var desafio = await _desafioRepository.GetById(id);
            return _mapper.Map<DesafioDto>(desafio);
        }

        public bool Validate( DesafioInsertDto desafioInsertDto )
        {
            throw new NotImplementedException();
        }
        public bool Validate(InsertDesafioImageDto insertDesafioImageDto )
        {
           if(_desafioRepository.Search(d => d.IdDesafio == insertDesafioImageDto.Id).Count() == 0)
            {
                Erros.Add("El desafio no existe");
                return true;
            }
            return true;
        }

        public bool Validate( int id )
        {
            if(_desafioRepository.Search(d => d.IdDesafio == id).Count() == 0)
            {
                Erros.Add("El desafio no existe");
                return false;
            }
            return true;
        }

        public async Task<DesafioDto> AddImage( InsertDesafioImageDto insertDesafioImageDto )
        {
            await _desafioRepository.AddImage(insertDesafioImageDto);
            await _desafioRepository.SaveChangesAsync();
            var desafio = _desafioRepository.Search(d => d.IdDesafio == insertDesafioImageDto.Id).FirstOrDefault();
            return _mapper.Map<DesafioDto>(desafio);
        }

        public async Task<Desafio> GetImage( int id )
        {
            var desafio = _desafioRepository.Search(d => d.IdDesafio == id).FirstOrDefault();
            if (desafio == null || desafio.ImagenComprob == null)
            {
                return null;
            }
            return _mapper.Map<Desafio>(desafio);
        }

        public string GetImageMimeType( byte[] imageData )
        {
            if (imageData.Length >= 4)
            {
                if (imageData[0] == 0xFF && imageData[1] == 0xD8)
                    return "image/jpeg";

                if (imageData[0] == 0x89 && imageData[1] == 0x50 &&
                    imageData[2] == 0x4E && imageData[3] == 0x47)
                    return "image/png";

                if (imageData[0] == 0x47 && imageData[1] == 0x49 &&
                    imageData[2] == 0x46)
                    return "image/gif";
            }
            return "application/octet-stream";
        }

        public IEnumerable<DesafioDto> SearchByTituloOrMeta(string search)
        {
            var resultados = _desafioRepository.SearchByTituloOrMeta(search);
            return resultados.Select(d => _mapper.Map<DesafioDto>(d));
        }

        public DesafioProgresoDto GetProgreso(int idDesafio)
        {
            var desafio = _desafioRepository.Search(d => d.IdDesafio == idDesafio).FirstOrDefault();
            if (desafio == null) return null;

            var participaciones = _desafioRepository.GetParticipacionesByDesafio(idDesafio);
            int total = participaciones.Count();
            int completados = participaciones.Count(p => p.Progreso == "100" || p.Progreso.ToLower() == "completado");

            double porcentaje = total == 0 ? 0 : (double)completados / total * 100;

            return new DesafioProgresoDto
            {
                IdDesafio = desafio.IdDesafio,
                Titulo = desafio.Titulo,
                TotalParticipantes = total,
                Completados = completados,
                PorcentajeCompletado = porcentaje
            };
        }

        public async Task Update( DesafioUpdateDto dto, int id )
        {
            var desafio = await _desafioRepository.GetByIdAsync(id);
            desafio.Descripcion = dto.Descripcion;
            desafio.Titulo = dto.Titulo;
            desafio.Meta = dto.Meta;
            desafio.FechaFin = dto.FechaFin;
            desafio.FechaInicio = dto.FechaInicio;
            await _desafioRepository.UpdateAsync(desafio);
            await _desafioRepository.SaveChangesAsync();
        }
    }
}
