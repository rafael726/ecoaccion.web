namespace ecoaccion.Core.DTOs.Desafio
{
    public class InsertDesafioImageDto
    {
        public required int Id { get; set; }
        public required IFormFile ImagenComprob { get; set; }
    }
}
