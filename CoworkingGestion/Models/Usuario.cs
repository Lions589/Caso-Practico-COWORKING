using System;
using System.Collections.Generic;

namespace CoworkingGestion.Models;

public partial class Usuario
{
    public int IdUsuario { get; set; }

    public string Ncedula { get; set; } = null!;

    public string Nombre { get; set; } = null!;

    public string Apellido { get; set; } = null!;

    public string Telefono { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Rol { get; set; } = null!;

    public virtual ICollection<Membresia> Membresia { get; set; } = new List<Membresia>();

    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}
