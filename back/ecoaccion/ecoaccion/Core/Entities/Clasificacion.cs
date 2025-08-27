namespace ecoaccion.Core.Entities
{
    public class Clasificacion
    {
        public int IdClasif { get; set; }
        public int IdUsuario { get; set; }
        public int? PuntosTotales { get; set; }
        public int? DesafiosCumplidos { get; set; }

        public Usuario Usuario { get; set; }
    }
}
