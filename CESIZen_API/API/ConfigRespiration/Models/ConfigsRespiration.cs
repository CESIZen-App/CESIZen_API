using System;
using System.Collections.Generic;
using CESIZen_API.API.Exercice.Models;

namespace CESIZen_API.API.ConfigRespiration.Models;

public partial class ConfigsRespirationModel
{
    public int Id { get; set; }

    public int ExerciceId { get; set; }

    public int TempsInspire { get; set; }

    public int TempsExpire { get; set; }

    public int? TempsPause { get; set; }

    public int NombreCycles { get; set; }

    public virtual ExerciceModel Exercice { get; set; } = null!;
}