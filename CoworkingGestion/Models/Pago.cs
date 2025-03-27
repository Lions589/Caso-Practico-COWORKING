using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class Pago
{
    public int IdPago { get; set; }

    public int? IdReserva { get; set; }

    public int? IdMembresia { get; set; }

    public decimal Monto { get; set; }

    public DateTime? FechaPago { get; set; }

    public string MetodoPago { get; set; } = null!;

    public virtual Membresia? IdMembresiaNavigation { get; set; }

    public virtual Reserva? IdReservaNavigation { get; set; }
}
