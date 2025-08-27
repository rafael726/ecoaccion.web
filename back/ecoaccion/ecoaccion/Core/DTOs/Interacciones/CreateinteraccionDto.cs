namespace ecoaccion.Core.DTOs.Interacciones
{
    public class CreateinteraccionDto
    {
        public int? IdAdmin { get; set; }
        public int? IdUsuario { get; set; }
        public string Tipo { get; set; }
        public string Mensaje { get; set; }
    }
}
