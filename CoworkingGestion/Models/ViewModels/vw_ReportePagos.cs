namespace CoworkingGestion.Models
{
    public class vw_ReportePagos
    {
        public int IdPago { get; set; }
        public int IdReserva { get; set; }
        public decimal Monto { get; set; }
        public DateTime FechaPago { get; set; }
        public string MetodoPago { get; set; }
        public string EstadoReserva { get; set; }
        public decimal CostoReserva { get; set; }
        public string NombreUsuario { get; set; }
    }
}
