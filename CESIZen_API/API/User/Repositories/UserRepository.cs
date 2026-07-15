// Implémentation du repository utilisateur.
// Hérite de BaseRepository pour les opérations CRUD génériques.
// Ajoute deux méthodes de recherche par email, dont une avec eager loading du rôle.

using CESIZen_API.API.User.Models;
using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CESIZen_API.API.User.Repositories
{
    public class UserRepository : BaseRepository<UserModel>, IUserRepository
    {
        public UserRepository(MyDbContext context) : base(context) { }

        /// <summary>
        /// Recherche par email sans jointure (rapide, utilisé pour la vérification d'unicité).
        /// </summary>
        public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        /// <summary>
        /// Recherche par email avec chargement du rôle (Include).
        /// Nécessaire lors du login pour accéder à Role.Libelle et l'inclure dans le JWT.
        /// </summary>
        public async Task<UserModel?> GetByEmailWithRoleAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
