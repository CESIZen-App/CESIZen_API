// Implémentation du repository Information.
// Hérite de BaseRepository pour les opérations CRUD génériques.

using CESIZen_API.API.Information.Models;
using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Repositories;

namespace CESIZen_API.API.Information.Repositories
{
    public class InformationRepository : BaseRepository<InformationModel>, IInformationRepository
    {
        public InformationRepository(MyDbContext context) : base(context) { }

        /// <summary>Filtre les pages publiées (IsPublished == true) via l'expression lambda.</summary>
        public async Task<List<InformationModel>> GetPublishedAsync(CancellationToken cancellationToken = default)
        {
            return await ListAsync(i => i.IsPublished, cancellationToken);
        }
    }
}
