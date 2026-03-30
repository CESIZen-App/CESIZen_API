using CESIZen_API.Shared.Repositories;
using ExerciceModel = CESIZen_API.API.Exercice.Models.ExerciceModel;

namespace CESIZen_API.API.Exercice.Repositories
{
    public interface IExerciceRepository : IBaseRepository<ExerciceModel>
    {
        Task<List<ExerciceModel>> GetByCreateurAsync(int createurId, CancellationToken cancellationToken = default);
        Task<List<ExerciceModel>> GetPublicAsync(CancellationToken cancellationToken = default);
    }
}