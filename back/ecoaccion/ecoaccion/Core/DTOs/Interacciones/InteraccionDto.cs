namespace ecoaccion.Core.DTOs.Interacciones
{
    public class InteraccionDto
    {
        public int? IdAdmin { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Tiempo { get; set; }
        public string Tipo { get; set; }
        public string Mensaje { get; set; }
    }
}
