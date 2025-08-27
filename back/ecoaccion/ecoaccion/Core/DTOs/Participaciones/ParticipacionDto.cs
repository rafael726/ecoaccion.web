namespace ecoaccion.Core.DTOs.Participaciones
{
    public class ParticipacionDto
    {
        public int IdPart { get; set; }
        public int IdUsuario { get; set; }
        public int IdDesafio { get; set; }
        public string? TituloDesafio { get; set; }
        public string Progreso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string? Evidencia { get; set; }
    }
}
