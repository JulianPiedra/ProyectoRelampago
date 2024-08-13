using System;
using System.Collections.Generic;

namespace ProyectoRelampago.Models;

public partial class Horarios
{
    public int HorarioId { get; set; }

    public int HoraEntrada { get; set; }

    public int HoraSalida { get; set; }

    public virtual ICollection<Usuarios> Usuarios { get; set; } = new List<Usuarios>();
}
