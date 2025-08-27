namespace ecoaccion.Core.Entities
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }
        public int? Puntos { get; set; }

        public ICollection<Clasificacion> Clasificaciones { get; set; }
        public ICollection<Interaccion> Interacciones { get; set; }
        public ICollection<Participacion> Participaciones { get; set; }
    }
}
