// Interface du repository de base générique.
// Définit les opérations CRUD et de requêtage communes à toutes les entités.
// Les repositories spécifiques (UserRepository, etc.) héritent de BaseRepository<T>
// qui implémente cette interface, et peuvent y ajouter leurs propres méthodes.

using System.Linq.Expressions;

namespace CESIZen_API.Shared.Repositories;

public interface IBaseRepository<TModel> where TModel : class
{
    /// <summary>Ajoute une entité en base et retourne l'entité persistée (avec son Id généré).</summary>
    Task<TModel> AddAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>Met à jour une entité existante en base.</summary>
    Task UpdateAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>Supprime une entité de la base.</summary>
    Task DeleteAsync(TModel model, CancellationToken cancellationToken = default);

    /// <summary>Recherche une entité par sa clé primaire. Retourne null si introuvable.</summary>
    Task<TModel?> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;

    /// <summary>Retourne toutes les entités de la table.</summary>
    Task<List<TModel>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>Retourne les entités correspondant au prédicat donné.</summary>
    Task<List<TModel>> ListAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Retourne true si au moins une entité correspond au prédicat.</summary>
    Task<bool> AnyAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Compte les entités correspondant au prédicat.</summary>
    Task<int> CountAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Compte toutes les entités de la table.</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Retourne la première entité correspondant au prédicat, ou null.</summary>
    Task<TModel?> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
}
