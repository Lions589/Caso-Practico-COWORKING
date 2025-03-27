namespace CoworkingGestion.Models
{
    public class vw_ReporteReservas
    {
        public int IdReserva { get; set; }
        public string NombreUsuario { get; set; } // Nombre completo del usuario
        public string Espacio { get; set; }
        public DateTime? FechaReserva { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string Estado { get; set; }
        public decimal Costo { get; set; }
        public string? CodigoQR { get; set; }

    }
}
