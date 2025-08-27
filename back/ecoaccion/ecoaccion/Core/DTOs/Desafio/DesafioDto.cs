namespace ecoaccion.Core.DTOs.Desafio
{
    public class DesafioDto
    {
        public int IdDesafio { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Meta { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string? ImagenComprob { get; set; }
        public int IdAdmin { get; set; }
    }
}
