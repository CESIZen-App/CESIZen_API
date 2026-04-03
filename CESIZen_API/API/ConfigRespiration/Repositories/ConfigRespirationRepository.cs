using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Repositories;
using ConfigModel = CESIZen_API.API.ConfigRespiration.Models.ConfigsRespirationModel;

namespace CESIZen_API.API.ConfigRespiration.Repositories
{
    public class ConfigRespirationRepository : BaseRepository<ConfigModel>, IConfigRespirationRepository
    {
        public ConfigRespirationRepository(MyDbContext context) : base(context) { }

        public async Task<List<ConfigModel>> GetByExerciceAsync(int exerciceId, CancellationToken cancellationToken = default)
        {
            return await ListAsync(c => c.ExerciceId == exerciceId, cancellationToken);
        }
    }
}