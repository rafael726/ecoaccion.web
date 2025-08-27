namespace ecoaccion.Core.Entities
{
    public class Participacion
    {
        public int IdPart { get; set; }
        public int IdUsuario { get; set; }
        public int IdDesafio { get; set; }
        public string Progreso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public byte[]? Evidencia { get; set; }
        public Usuario Usuario { get; set; }
        public Desafio Desafio { get; set; }
    }
}
