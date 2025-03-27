using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class VwAnalisisUso
{
    public string Espacio { get; set; } = null!;

    public int? TotalReservas { get; set; }

    public decimal? IngresosTotales { get; set; }

    public DateTime? PrimerUso { get; set; }

    public DateTime? UltimoUso { get; set; }
}
