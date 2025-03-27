using System.Collections.Generic;

namespace CoworkingGestion.Models
{
    public class DashboardViewModel
    {
        public List<Reserva> Reservas { get; set; } = new List<Reserva>();
        public List<Membresia> Membresias { get; set; } = new List<Membresia>();
    }
}
