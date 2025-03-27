using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class Membresia
{
    public int IdMembresia { get; set; }

    public int IdUsuario { get; set; }

    public string NombrePlan { get; set; } = null!;

    public decimal Precio { get; set; }

    public string Duracion { get; set; } = null!;

    public string? Descripcion { get; set; }

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;

    public virtual ICollection<Pago> Pagos { get; set; } = new List<Pago>();
}
