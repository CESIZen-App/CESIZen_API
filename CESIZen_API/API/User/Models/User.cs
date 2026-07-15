// Modèle de données de l'entité Utilisateur.
// Correspond à la table "users" en base de données PostgreSQL.
// Le mot de passe est stocké au format "hash_hex:sel_hex" (jamais en clair).
// Les propriétés de navigation (Role, Exercices) permettent les jointures EF Core.

using System;
using System.Collections.Generic;
using CESIZen_API.API.Role.Models;
using CESIZen_API.API.Exercice.Models;

namespace CESIZen_API.API.User.Models;

public partial class UserModel
{
    /// <summary>Identifiant unique auto-incrémenté.</summary>
    public int Id { get; set; }

    /// <summary>Nom / pseudonyme de l'utilisateur (optionnel).</summary>
    public string? Nom { get; set; }

    /// <summary>Adresse email unique, utilisée comme identifiant de connexion.</summary>
    public string Email { get; set; } = null!;

    /// <summary>Mot de passe haché au format "hash_hex:sel_hex" (PBKDF2-SHA512).</summary>
    public string Password { get; set; } = null!;

    /// <summary>Identifiant du rôle : 1 = ADMIN, 2 = USER.</summary>
    public int RoleId { get; set; }

    /// <summary>Exercices créés par cet utilisateur (relation 1:N).</summary>
    public virtual ICollection<ExerciceModel> Exercices { get; set; } = new List<ExerciceModel>();

    /// <summary>Rôle associé à cet utilisateur (relation N:1).</summary>
    public virtual RoleModel Role { get; set; } = null!;
}
