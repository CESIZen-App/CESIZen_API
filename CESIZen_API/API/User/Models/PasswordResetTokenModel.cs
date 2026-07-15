// Modèle du token de réinitialisation de mot de passe.
// Correspond à la table "password_reset_tokens" en base de données.
// Chaque token est à usage unique (Used) et expire après 1 heure (ExpiresAt).
// La vérification dans UserService filtre sur : token correspondant + non utilisé + non expiré.

using CESIZen_API.API.User.Models;

namespace CESIZen_API.API.User.Models;

public class PasswordResetTokenModel
{
    /// <summary>Identifiant unique auto-incrémenté.</summary>
    public int Id { get; set; }

    /// <summary>Identifiant de l'utilisateur concerné par la réinitialisation.</summary>
    public int UserId { get; set; }

    /// <summary>Token aléatoire de 64 caractères hex (32 octets via RandomNumberGenerator).</summary>
    public string Token { get; set; } = null!;

    /// <summary>Date d'expiration du token (1 heure après sa création).</summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>Indique si le token a déjà été utilisé (consommé après une réinitialisation réussie).</summary>
    public bool Used { get; set; }

    /// <summary>Utilisateur propriétaire du token (propriété de navigation EF Core).</summary>
    public virtual UserModel User { get; set; } = null!;
}
