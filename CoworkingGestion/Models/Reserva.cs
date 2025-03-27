using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class Reserva
{
    public int IdReserva { get; set; }

    public int IdUsuario { get; set; }

    public int IdEspacio { get; set; }

    public DateTime? FechaReserva { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string Estado { get; set; } = null!;

    public decimal Costo { get; set; }

    public string? CodigoQr { get; set; }

    public virtual Espacio IdEspacioNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
