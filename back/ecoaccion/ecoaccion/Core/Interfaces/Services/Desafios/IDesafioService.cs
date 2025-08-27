using ecoaccion.Core.DTOs.Desafio;
using ecoaccion.Core.DTOs.User;
using ecoaccion.Core.Entities;

namespace ecoaccion.Core.Interfaces.Services.Desafios
{
    public interface IDesafioService
    {
        public List<string> Erros { get; }
        Task<DesafioDto> Add( DesafioInsertDto desafioInsertDto );
        bool Delete( int id );
        Task<DesafioDto> AddImage( InsertDesafioImageDto insertDesafioImageDto );
        Task<Desafio> GetImage( int id );
        string GetImageMimeType( byte[] imageData );
        Task<DesafioDto> GetById( int id );
        Task<IEnumerable<DesafioDto>> GetAll();
        bool Validate( DesafioInsertDto userInsertDto );
        bool Validate(InsertDesafioImageDto imageInsertDto );
        bool Validate( int id );
        public IEnumerable<DesafioDto> SearchByTituloOrMeta( string search );
        DesafioProgresoDto GetProgreso( int idDesafio );
        Task Update( DesafioUpdateDto dto, int id );
    }
}
