using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class Espacio
{
    public int IdEspacio { get; set; }

    public string Nombre { get; set; } = null!;

    public string Ubicacion { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int? Capacidad { get; set; }

    public string? Estado { get; set; }
    public decimal Costo { get; set; }

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
