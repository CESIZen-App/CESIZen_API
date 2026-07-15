// Interface du repository Exercice.
// Étend IBaseRepository avec des requêtes filtrées par créateur et par visibilité.

using CESIZen_API.Shared.Repositories;
using ExerciceModel = CESIZen_API.API.Exercice.Models.ExerciceModel;

namespace CESIZen_API.API.Exercice.Repositories
{
    public interface IExerciceRepository : IBaseRepository<ExerciceModel>
    {
        /// <summary>Retourne tous les exercices créés par un utilisateur donné.</summary>
        Task<List<ExerciceModel>> GetByCreateurAsync(int createurId, CancellationToken cancellationToken = default);

        /// <summary>Retourne tous les exercices marqués IsPublic = true (visibles par tous).</summary>
        Task<List<ExerciceModel>> GetPublicAsync(CancellationToken cancellationToken = default);
    }
}
