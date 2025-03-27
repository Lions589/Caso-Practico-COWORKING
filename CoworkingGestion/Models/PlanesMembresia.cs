using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class PlanesMembresia
{
    public int IdPlan { get; set; }

    public string NombrePlan { get; set; } = null!;

    public decimal Precio { get; set; }

    public string Duracion { get; set; } = null!;

    public string? Descripcion { get; set; }
}
