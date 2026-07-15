// Implémentation du repository Exercice.
// Hérite de BaseRepository pour les opérations CRUD génériques.

using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Repositories;
using ExerciceModel = CESIZen_API.API.Exercice.Models.ExerciceModel;

namespace CESIZen_API.API.Exercice.Repositories
{
    public class ExerciceRepository : BaseRepository<ExerciceModel>, IExerciceRepository
    {
        public ExerciceRepository(MyDbContext context) : base(context) { }

        /// <summary>Filtre les exercices par créateur via l'expression lambda.</summary>
        public async Task<List<ExerciceModel>> GetByCreateurAsync(int createurId, CancellationToken cancellationToken = default)
        {
            return await ListAsync(e => e.CreateurId == createurId, cancellationToken);
        }

        /// <summary>Filtre les exercices publics (IsPublic == true).</summary>
        public async Task<List<ExerciceModel>> GetPublicAsync(CancellationToken cancellationToken = default)
        {
            return await ListAsync(e => e.IsPublic == true, cancellationToken);
        }
    }
}
