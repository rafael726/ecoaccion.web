namespace ecoaccion.Core.Entities
{
    public class Interaccion
    {
        public int IdInteraccion { get; set; }
        public int? IdAdmin { get; set; }
        public int? IdUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan Tiempo { get; set; }
        public string Tipo { get; set; }
        public string Mensaje { get; set; }

        public Administrador Administrador { get; set; }
        public Usuario Usuario { get; set; }
    }
}
