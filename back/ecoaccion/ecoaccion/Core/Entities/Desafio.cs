namespace ecoaccion.Core.Entities
{
    public class Desafio
    {
        public int IdDesafio { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public string Meta { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public byte[]? ImagenComprob { get; set; }
        public int IdAdmin { get; set; }

        public Administrador Administrador { get; set; }
        public ICollection<Participacion> Participaciones { get; set; }
    }
}
