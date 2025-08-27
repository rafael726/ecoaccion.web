namespace ecoaccion.Core.DTOs.Desafio
{
    public class DesafioProgresoDto
    {
        public int IdDesafio { get; set; }
        public string Titulo { get; set; }
        public int TotalParticipantes { get; set; }
        public int Completados { get; set; }
        public double PorcentajeCompletado { get; set; }
    }
}
