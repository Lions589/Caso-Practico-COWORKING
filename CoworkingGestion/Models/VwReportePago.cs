using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class VwReportePago
{
    public int IdPago { get; set; }

    public int IdReserva { get; set; }

    public decimal Monto { get; set; }

    public DateTime? FechaPago { get; set; }

    public string MetodoPago { get; set; } = null!;

    public string EstadoReserva { get; set; } = null!;

    public decimal CostoReserva { get; set; }

    public string NombreUsuario { get; set; } = null!;
}
