namespace CoworkingGestion.Models
{
    public class vw_AnalisisUso
    {
        public string Espacio { get; set; }
        public int TotalReservas { get; set; }
        public decimal IngresosTotales { get; set; }
        public DateTime PrimerUso { get; set; }
        public DateTime UltimoUso { get; set; }
    }
}
