using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class VwReporteReserva
{
    public int IdReserva { get; set; }

    public string NombreUsuario { get; set; } = null!;

    public string Espacio { get; set; } = null!;

    public DateTime? FechaReserva { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime FechaFin { get; set; }

    public string Estado { get; set; } = null!;

    public decimal Costo { get; set; }

    public string? CodigoQr { get; set; }
}
