using CESIZen_API.API.Information.Models;
using CESIZen_API.Shared.Repositories;

namespace CESIZen_API.API.Information.Repositories
{
    public interface IInformationRepository : IBaseRepository<InformationModel>
    {
        Task<List<InformationModel>> GetPublishedAsync(CancellationToken cancellationToken = default);
    }
}
