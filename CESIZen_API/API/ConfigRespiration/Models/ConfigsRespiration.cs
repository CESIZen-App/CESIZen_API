using System;
using System.Collections.Generic;

namespace CESIZen_API.Models;

public partial class ConfigsRespiration
{
    public int Id { get; set; }

    public int ExerciceId { get; set; }

    public int TempsInspire { get; set; }

    public int TempsExpire { get; set; }

    public int? TempsPause { get; set; }

    public int NombreCycles { get; set; }

    public virtual Exercice Exercice { get; set; } = null!;
}
