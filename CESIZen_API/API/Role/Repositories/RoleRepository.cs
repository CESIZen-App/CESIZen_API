// Implémentation du repository Rôle.
// Hérite de BaseRepository pour les opérations CRUD génériques.

using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Repositories;
using RoleModel = CESIZen_API.API.Role.Models.RoleModel;

namespace CESIZen_API.API.Role.Repositories
{
    public class RoleRepository : BaseRepository<RoleModel>, IRoleRepository
    {
        public RoleRepository(MyDbContext context) : base(context) { }

        /// <summary>Recherche un rôle par son libellé exact.</summary>
        public async Task<RoleModel?> GetByLibelleAsync(string libelle, CancellationToken cancellationToken = default)
        {
            return await FirstOrDefaultAsync(r => r.Libelle == libelle, cancellationToken);
        }
    }
}
