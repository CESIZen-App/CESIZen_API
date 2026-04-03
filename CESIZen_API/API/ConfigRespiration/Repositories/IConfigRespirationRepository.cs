using CESIZen_API.Shared.Repositories;
using ConfigModel = CESIZen_API.API.ConfigRespiration.Models.ConfigsRespirationModel;

namespace CESIZen_API.API.ConfigRespiration.Repositories
{
    public interface IConfigRespirationRepository : IBaseRepository<ConfigModel>
    {
        Task<List<ConfigModel>> GetByExerciceAsync(int exerciceId, CancellationToken cancellationToken = default);
    }
}