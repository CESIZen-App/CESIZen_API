// Interface du repository ConfigRespiration.
// Étend IBaseRepository avec une requête filtrée par exercice.

using CESIZen_API.Shared.Repositories;
using ConfigModel = CESIZen_API.API.ConfigRespiration.Models.ConfigsRespirationModel;

namespace CESIZen_API.API.ConfigRespiration.Repositories
{
    public interface IConfigRespirationRepository : IBaseRepository<ConfigModel>
    {
        /// <summary>Retourne toutes les configurations rattachées à un exercice donné.</summary>
        Task<List<ConfigModel>> GetByExerciceAsync(int exerciceId, CancellationToken cancellationToken = default);
    }
}
