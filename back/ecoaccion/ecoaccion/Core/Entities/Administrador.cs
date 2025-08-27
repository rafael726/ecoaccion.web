namespace ecoaccion.Core.Entities
{
    public class Administrador
    {
        public int IdAdmin { get; set; }
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Contrasena { get; set; }

        public ICollection<Desafio> Desafios { get; set; }
        public ICollection<Interaccion> Interacciones { get; set; }
    }
}
