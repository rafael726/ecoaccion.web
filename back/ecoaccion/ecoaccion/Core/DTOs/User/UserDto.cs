using ecoaccion.Core.DTOs.Admin;

namespace ecoaccion.Core.DTOs.User
{
    public class UserDto
    {
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public int IdUsuario { get; set; }
        public int? Puntos { get; set; }
    }
}
