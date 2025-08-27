namespace ecoaccion.Core.DTOs.Participaciones
{
    public class ParticipicacionInsertDto
    {
        public int IdUsuario { get; set; }
        public int IdDesafio { get; set; }
        public string Progreso { get; set; }
        public DateTime FechaRegistro { get; set; }
        public IFormFile Evidencia { get; set; }
    }
}
