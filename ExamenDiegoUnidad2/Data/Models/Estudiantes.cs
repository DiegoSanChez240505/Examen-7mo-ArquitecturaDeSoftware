using System;
using System.Collections.Generic;

namespace ExamenDiegoUnidad2.Data.Models;

public partial class Estudiantes
{
    public int IdEstudiante { get; set; }

    public string Matricula { get; set; } = null!;

    public string NombreCompleto { get; set; } = null!;

    public sbyte Semestre { get; set; }

    public decimal Promedio { get; set; }

    public int IdCarreraFk { get; set; }

    public int IdTipoEstudianteFk { get; set; }

    public virtual Carreras IdCarreraFkNavigation { get; set; } = null!;

    public virtual TiposEstudiante IdTipoEstudianteFkNavigation { get; set; } = null!;
}
