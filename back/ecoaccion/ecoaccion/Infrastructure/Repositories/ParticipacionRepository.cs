using ecoaccion.Core.Entities;
using ecoaccion.Core.Interfaces.Repositories;
using ecoaccion.Infrastructure.Common;
using ecoaccion.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;
using ecoaccion.Core.DTOs.Participaciones;

namespace ecoaccion.Infrastructure.Repositories
{
    public class ParticipacionRepository(AppDbContext context):BaseRepository<Participacion>(context), IParticipacionRepository
    {
        public async Task<bool> UpdateParticipacionProgresoAsync(ParticipacionProgresoUpdateDto dto)
        {
            var participacion = _context.Participaciones
                .FirstOrDefault(p => p.IdUsuario == dto.IdUsuario && p.IdPart == dto.IdPart);

            if (participacion == null) return false;

            participacion.Progreso = dto.Progreso?.Trim().ToLower();
            participacion.FechaRegistro = DateTime.UtcNow;

            // Procesar la imagen de evidencia si se envía
            if (dto.evidencia != null)
            {
                using var img = await Image.LoadAsync(dto.evidencia.OpenReadStream());
                img.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(512, 512),
                    Mode = ResizeMode.Max
                }));
                using var memoryStream = new MemoryStream();
                await img.SaveAsJpegAsync(memoryStream);
                participacion.Evidencia = memoryStream.ToArray();
            }

            _context.Participaciones.Update(participacion);
            await _context.SaveChangesAsync();
            return true;
        }

        public IEnumerable<Participacion> GetHistorialParticipacion( int idUsuario )
        {
            return _context.Participaciones
                .Where(p => p.IdUsuario == idUsuario)
                .OrderByDescending(p => p.FechaRegistro)
                .ToList();
        }

        public async Task<IEnumerable<Participacion>> GetHistorialParticipacionAsync( int idUsuario )
        {
            var participaciones = await _context.Participaciones
                .Include(p => p.Desafio)
                .Where(p => p.IdUsuario == idUsuario)
                .OrderByDescending(p => p.FechaRegistro)
                .ToListAsync();

            return participaciones;
        }
        public async Task<bool> CargarEvidenciaAsync(int idUsuario, int idDesafio, byte[] evidencia)
        {
            var participacion = _context.Participaciones
                .FirstOrDefault(p => p.IdUsuario == idUsuario && p.IdDesafio == idDesafio);

            if (participacion == null) return false;

            participacion.Evidencia = evidencia;
            participacion.FechaRegistro = DateTime.UtcNow;
            _context.Participaciones.Update(participacion);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> AddParticipacionAsync( ParticipicacionInsertDto participicacionInsertDto )
        {
            byte[] evidenciaBytes = null;
            if (participicacionInsertDto.Evidencia != null)
            {
                using var img = await Image.LoadAsync(participicacionInsertDto.Evidencia.OpenReadStream());
                img.Mutate(x => x.Resize(new ResizeOptions
                {
                    Size = new Size(512, 512),
                    Mode = ResizeMode.Max
                }));
                using var memoryStream = new MemoryStream();
                await img.SaveAsJpegAsync(memoryStream);
                evidenciaBytes = memoryStream.ToArray();
            }

            var participacion = new Participacion
            {
                IdUsuario = participicacionInsertDto.IdUsuario,
                IdDesafio = participicacionInsertDto.IdDesafio,
                Progreso = participicacionInsertDto.Progreso?.Trim().ToLower(),
                FechaRegistro = DateTime.UtcNow,
                Evidencia = evidenciaBytes
            };

            await _context.Participaciones.AddAsync(participacion);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
        public new IEnumerable<Participacion> Search( Func<Participacion, bool> filer )
            => _context.Participaciones.Where(filer).ToList();
        public IEnumerable<Usuario> Search( Func<Usuario, bool> filer )
            => _context.Usuarios.Where(filer).ToList();
        public IEnumerable<Desafio> Search( Func<Desafio, bool> filer )
            => _context.Desafios.Where(filer).ToList();

    }
}
