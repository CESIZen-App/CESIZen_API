// Interface du repository utilisateur.
// Étend IBaseRepository avec des méthodes de recherche spécifiques à l'entité User.

using CESIZen_API.API.User.Models;
using CESIZen_API.Shared.Repositories;

namespace CESIZen_API.API.User.Repositories
{
    public interface IUserRepository : IBaseRepository<UserModel>
    {
        /// <summary>
        /// Recherche un utilisateur par son email (sans charger le rôle).
        /// Utilisé pour vérifier l'unicité de l'email lors de l'inscription.
        /// </summary>
        Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recherche un utilisateur par son email en incluant son rôle (relation eager loading).
        /// Utilisé lors de la connexion pour obtenir le libellé du rôle (inclus dans le JWT).
        /// </summary>
        Task<UserModel?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default);
    }
}
