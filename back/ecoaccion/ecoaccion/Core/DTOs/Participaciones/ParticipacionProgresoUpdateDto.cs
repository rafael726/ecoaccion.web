namespace ecoaccion.Core.DTOs.Participaciones
{
    public class ParticipacionProgresoUpdateDto
    {
        public int IdUsuario { get; set; }
        public int IdPart { get; set; }
        public string Progreso { get; set; }
        public IFormFile? evidencia { get; set; }
    }
}