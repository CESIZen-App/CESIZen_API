// Interface du repository Information.
// Étend IBaseRepository avec une requête filtrée sur les pages publiées.

using CESIZen_API.API.Information.Models;
using CESIZen_API.Shared.Repositories;

namespace CESIZen_API.API.Information.Repositories
{
    public interface IInformationRepository : IBaseRepository<InformationModel>
    {
        /// <summary>Retourne uniquement les pages dont IsPublished = true (visibles par tous).</summary>
        Task<List<InformationModel>> GetPublishedAsync(CancellationToken cancellationToken = default);
    }
}
