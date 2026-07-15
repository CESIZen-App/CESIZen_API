// Modèle de données d'une configuration de respiration.
// Chaque configuration est rattachée à un exercice et définit les temps de chaque phase du cycle.

using System;
using System.Collections.Generic;
using CESIZen_API.API.Exercice.Models;

namespace CESIZen_API.API.ConfigRespiration.Models;

public partial class ConfigsRespirationModel
{
    /// <summary>Identifiant unique de la configuration (clé primaire).</summary>
    public int Id { get; set; }

    /// <summary>Identifiant de l'exercice auquel cette configuration est rattachée (clé étrangère).</summary>
    public int ExerciceId { get; set; }

    /// <summary>Durée de la phase d'inspiration en secondes (obligatoire, ≥ 1).</summary>
    public int TempsInspire { get; set; }

    /// <summary>Durée de la phase d'expiration en secondes (obligatoire, ≥ 1).</summary>
    public int TempsExpire { get; set; }

    /// <summary>
    /// Durée de la pause entre inspiration et expiration en secondes.
    /// Nullable : null ou 0 signifie qu'il n'y a pas de phase d'apnée (technique 5-5 par exemple).
    /// </summary>
    public int? TempsPause { get; set; }

    /// <summary>Nombre de cycles à effectuer lors de l'exercice (obligatoire, ≥ 1).</summary>
    public int NombreCycles { get; set; }

    /// <summary>Navigation vers l'exercice parent.</summary>
    public virtual ExerciceModel Exercice { get; set; } = null!;
}
