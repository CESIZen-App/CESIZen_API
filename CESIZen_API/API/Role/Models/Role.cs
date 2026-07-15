// Modèle de l'entité Rôle.
// Correspond à la table "roles" en base de données.
// Deux rôles sont insérés par le script SQL d'initialisation :
//   - Id=1 : ADMIN
//   - Id=2 : USER

using System;
using System.Collections.Generic;
using CESIZen_API.API.User.Models;

namespace CESIZen_API.API.Role.Models;

public partial class RoleModel
{
    public int Id { get; set; }

    /// <summary>Libellé du rôle : "ADMIN" ou "USER".</summary>
    public string Libelle { get; set; } = null!;

    /// <summary>Utilisateurs ayant ce rôle (relation 1:N).</summary>
    public virtual ICollection<UserModel> Users { get; set; } = new List<UserModel>();
}
