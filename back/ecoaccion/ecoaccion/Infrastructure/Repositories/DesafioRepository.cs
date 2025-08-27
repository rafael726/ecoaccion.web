#nullable disable
using ecoaccion.Core.DTOs.Desafio;
using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Infrastructure.Common;
using ecoaccion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ecoaccion.Infrastructure.Repositories
{
    public class DesafioRepository( AppDbContext context ) : BaseRepository<Desafio>(context), IDesafioRepository
    {
        public async Task AddDesafioAsync( Desafio desafio ) 
            => await _context.Desafios.AddAsync( desafio);

        public async Task AddImage( InsertDesafioImageDto image )
        {
            var defasio = await _context.Desafios.FindAsync( image.Id );
            if (defasio != null) 
            {
                using var img = await Image.LoadAsync(image.ImagenComprob.OpenReadStream());
                img.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(512, 512),
                    Mode = ResizeMode.Max
                }));
                using var memoryStream = new MemoryStream();
                await image.ImagenComprob.CopyToAsync( memoryStream );
                await img.SaveAsJpegAsync( memoryStream );
                defasio.ImagenComprob = memoryStream.ToArray();
                defasio.FechaInicio = DateTime.SpecifyKind(defasio.FechaInicio, DateTimeKind.Utc);
                defasio.FechaFin = DateTime.SpecifyKind(defasio.FechaFin, DateTimeKind.Utc);
                _context.Desafios.Update(defasio);
            }
        }

        public void DeleteDesafioAsync( int id )
        {
            var desafio = _context.Desafios.Where(d => d.IdDesafio == id).FirstOrDefault();
            _context.Desafios.Remove(desafio);
        }

        public async Task<IEnumerable<Desafio>> GetAll() 
            => await _context.Desafios.ToListAsync();

        public async Task<Desafio?> GetById( int id )
        {
            var desafio = await _context.Desafios.FirstOrDefaultAsync(d => d.IdDesafio == id);
            if (desafio == null)
                return null;

            return desafio;
        }

        public IEnumerable<Desafio> SearchByTituloOrMeta( string search )
        {
            return _context.Desafios
                .Where(d => d.Titulo.Contains(search) || d.Meta.Contains(search))
                .ToList();
        }
        public IEnumerable<Participacion> GetParticipacionesByDesafio( int idDesafio )
        {
            return _context.Participaciones.Where(p => p.IdDesafio == idDesafio).ToList();
        }

        public async Task UpdateAsync( DesafioUpdateDto dto, int id )
        {
            var desafio = await _context.Desafios.FirstOrDefaultAsync(d => d.IdDesafio == id);
            _context.Desafios.Update(desafio);
        }
    }
}
