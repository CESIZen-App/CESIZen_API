// Interface du service utilisateur.
// Définit le contrat métier du module User :
// inscription, connexion, CRUD, gestion de rôle et réinitialisation de mot de passe.

using CESIZen_API.API.User.DTOs;

namespace CESIZen_API.API.User.Services
{
    public interface IUserService
    {
        /// <summary>Inscrit un nouvel utilisateur et retourne un JWT (auto-login).</summary>
        Task<AuthResponseDTO> RegisterAsync(RegisterDTO dto);

        /// <summary>Authentifie un utilisateur et retourne un JWT.</summary>
        Task<AuthResponseDTO> LoginAsync(LoginDTO dto);

        /// <summary>Retourne tous les utilisateurs (admin uniquement).</summary>
        Task<IEnumerable<UserResponseDTO>> GetAllAsync();

        /// <summary>Retourne un utilisateur par son identifiant.</summary>
        Task<UserResponseDTO?> GetByIdAsync(int id);

        /// <summary>Met à jour les informations d'un utilisateur (nom, email, mot de passe).</summary>
        Task<UserResponseDTO> UpdateAsync(int id, UpdateUserDTO dto);

        /// <summary>Supprime un utilisateur par son identifiant.</summary>
        Task DeleteAsync(int id);

        /// <summary>Génère et envoie un token de réinitialisation par email.</summary>
        Task ForgotPasswordAsync(ForgotPasswordDTO dto);

        /// <summary>Réinitialise le mot de passe via le token reçu par email.</summary>
        Task ResetPasswordAsync(ResetPasswordDTO dto);

        /// <summary>Change le rôle d'un utilisateur (ADMIN uniquement).</summary>
        Task<UserResponseDTO> ChangeRoleAsync(int id, ChangeRoleDTO dto);
    }
}
