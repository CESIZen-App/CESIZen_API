// Interface du repository Rôle.
// Étend IBaseRepository avec une méthode de recherche par libellé
// (utilisée pour vérifier l'unicité avant la création d'un rôle).

using CESIZen_API.Shared.Repositories;
using RoleModel = CESIZen_API.API.Role.Models.RoleModel;

namespace CESIZen_API.API.Role.Repositories
{
    public interface IRoleRepository : IBaseRepository<RoleModel>
    {
        /// <summary>
        /// Recherche un rôle par son libellé (insensible à la casse côté DB).
        /// Retourne null si aucun rôle ne correspond.
        /// </summary>
        Task<RoleModel?> GetByLibelleAsync(string libelle, CancellationToken cancellationToken = default);
    }
}
