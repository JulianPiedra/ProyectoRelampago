using System;
using System.Collections.Generic;

namespace ProyectoRelampago.Models;

public partial class Marcas
{
    public int MarcaId { get; set; }

    public string HoraEntrada { get; set; } = null!;

    public string? HoraSalida { get; set; }

    public DateTime? Fecha { get; set; }

    public int? Empleado { get; set; }

    public virtual Usuarios? EmpleadoNavigation { get; set; }
}
