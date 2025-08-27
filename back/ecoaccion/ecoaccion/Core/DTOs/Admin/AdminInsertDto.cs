namespace ecoaccion.Core.DTOs.Admin
{
    public class AdminInsertDto
    {
        public string NombreUsuario { get; set; }
        public string Correo { get; set; }
        public string Contraseña { get; set; }
        public string ConfirmarContraseña { get; set; }
    }
}
