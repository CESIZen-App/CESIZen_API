using CESIZen_API.Shared.Repositories;
using RoleModel = CESIZen_API.API.Role.Models.RoleModel;

namespace CESIZen_API.API.Role.Repositories
{
    public interface IRoleRepository : IBaseRepository<RoleModel>
    {
        Task<RoleModel?> GetByLibelleAsync(string libelle, CancellationToken cancellationToken = default);
    }
}