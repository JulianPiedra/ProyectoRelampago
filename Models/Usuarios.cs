using System;
using System.Collections.Generic;

namespace ProyectoRelampago.Models;

public partial class Usuarios
{
    public int UsuarioId { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Email { get; set; }

    public string? Contrasena { get; set; }

    public int? Horario { get; set; }

    public virtual Horarios? HorarioNavigation { get; set; }

    public virtual ICollection<Marcas> Marcas { get; set; } = new List<Marcas>();
}
