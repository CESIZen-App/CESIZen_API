// Modèle de données d'un exercice de respiration.
// Un exercice appartient à un créateur (utilisateur) et peut être public ou privé.
// Il peut posséder plusieurs configurations de respiration (ConfigsRespiration).

using System;
using System.Collections.Generic;
using CESIZen_API.API.ConfigRespiration.Models;
using CESIZen_API.API.User.Models;

namespace CESIZen_API.API.Exercice.Models;

public partial class ExerciceModel
{
    /// <summary>Identifiant unique de l'exercice (clé primaire).</summary>
    public int Id { get; set; }

    /// <summary>Titre de l'exercice (obligatoire, max 100 caractères).</summary>
    public string Titre { get; set; } = null!;

    /// <summary>Description optionnelle de l'exercice.</summary>
    public string? Description { get; set; }

    /// <summary>
    /// Indique si l'exercice est visible par tous les utilisateurs.
    /// false (ou null) = exercice privé, visible uniquement par son créateur.
    /// </summary>
    public bool? IsPublic { get; set; }

    /// <summary>Identifiant de l'utilisateur créateur de l'exercice (clé étrangère).</summary>
    public int CreateurId { get; set; }

    /// <summary>
    /// Configurations de respiration associées à cet exercice.
    /// Chaque configuration définit les temps d'inspiration, pause et expiration.
    /// </summary>
    public virtual ICollection<ConfigsRespirationModel> ConfigsRespirations { get; set; } = new List<ConfigsRespirationModel>();

    /// <summary>Navigation vers l'utilisateur créateur de l'exercice.</summary>
    public virtual UserModel Createur { get; set; } = null!;
}
