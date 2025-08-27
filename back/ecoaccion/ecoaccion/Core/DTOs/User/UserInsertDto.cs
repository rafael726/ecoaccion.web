using ecoaccion.Core.DTOs.Admin;

namespace ecoaccion.Core.DTOs.User
{
    public class UserInsertDto
    {
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string ConfirmarContraseña { get; set; }
    }
}
