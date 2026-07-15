// DTOs (Data Transfer Objects) du module Utilisateur.
// Séparent les données échangées avec le client des modèles internes.
// Le validateur [PasswordValidator] vérifie les règles de complexité du mot de passe.
// [EmailAddress] valide le format de l'email côté ASP.NET Core avant d'atteindre le service.

using System.ComponentModel.DataAnnotations;
using CESIZen_API.Shared.Validators;

namespace CESIZen_API.API.User.DTOs
{
    /// <summary>DTO d'inscription : données envoyées par l'utilisateur lors de la création de compte.</summary>
    public class RegisterDTO
    {
        [Required]
        public string Nom { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [PasswordValidator] // Vérifie les règles de complexité (longueur, chiffres, symboles, etc.)
        public string Password { get; set; } = null!;
    }

    /// <summary>DTO de connexion : identifiants envoyés pour obtenir un JWT.</summary>
    public class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }

    /// <summary>DTO de mise à jour de profil : tous les champs sont optionnels (PATCH partiel simulé).</summary>
    public class UpdateUserDTO
    {
        public string? Nom { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [PasswordValidator]
        public string? Password { get; set; }
    }

    /// <summary>DTO de réponse : données publiques d'un utilisateur (sans mot de passe).</summary>
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string Email { get; set; } = null!;
        /// <summary>Identifiant du rôle : 1 = ADMIN, 2 = USER.</summary>
        public int RoleId { get; set; }
    }

    /// <summary>DTO de demande de réinitialisation de mot de passe (envoi de l'email avec le token).</summary>
    public class ForgotPasswordDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
    }

    /// <summary>DTO de réinitialisation : token reçu par email + nouveau mot de passe.</summary>
    public class ResetPasswordDTO
    {
        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [PasswordValidator]
        public string NewPassword { get; set; } = null!;
    }

    /// <summary>DTO de changement de rôle : accepte uniquement 1 (ADMIN) ou 2 (USER).</summary>
    public class ChangeRoleDTO
    {
        [Required]
        [Range(1, 2, ErrorMessage = "RoleId doit être 1 (ADMIN) ou 2 (USER).")]
        public int RoleId { get; set; }
    }

    /// <summary>DTO de réponse d'authentification : token JWT + profil de l'utilisateur connecté.</summary>
    public class AuthResponseDTO
    {
        public string Token { get; set; } = null!;
        public UserResponseDTO User { get; set; } = null!;
    }
}
