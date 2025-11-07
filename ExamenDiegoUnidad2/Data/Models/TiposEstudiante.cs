using System;
using System.Collections.Generic;

namespace ExamenDiegoUnidad2.Data.Models;

public partial class TiposEstudiante
{
    public int IdTipoEstudiante { get; set; }

    public string NombreTipo { get; set; } = null!;

    public virtual ICollection<Estudiantes> Estudiantes { get; set; } = new List<Estudiantes>();
}
