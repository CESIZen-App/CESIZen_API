// Implémentation générique du repository de base.
// Fournit les opérations CRUD et de requêtage communes via Entity Framework Core.
// Les repositories spécifiques héritent de cette classe et peuvent surcharger
// les méthodes virtuelles ou en ajouter de nouvelles.

using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using CESIZen_API.Shared.Data;
using CESIZen_API.Shared.Extensions;

namespace CESIZen_API.Shared.Repositories;

public class BaseRepository<TModel> : IBaseRepository<TModel> where TModel : class
{
    // Contexte EF Core partagé (injecté par le conteneur DI)
    protected readonly MyDbContext _context;

    public BaseRepository(MyDbContext context)
    {
        _context = context;
    }

    /// <summary>Ajoute l'entité au DbSet et persiste les changements.</summary>
    public virtual async Task<TModel> AddAsync(TModel model, CancellationToken cancellationToken = default)
    {
        _context.Set<TModel>().Add(model);
        await SaveChangesAsync(cancellationToken);
        return model;
    }

    /// <summary>Marque l'entité comme modifiée et persiste les changements.</summary>
    public virtual async Task UpdateAsync(TModel model, CancellationToken cancellationToken = default)
    {
        _context.Set<TModel>().Update(model);
        await SaveChangesAsync(cancellationToken);
    }

    /// <summary>Supprime l'entité du DbSet et persiste les changements.</summary>
    public virtual async Task DeleteAsync(TModel model, CancellationToken cancellationToken = default)
    {
        _context.Set<TModel>().Remove(model);
        await SaveChangesAsync(cancellationToken);
    }

    /// <summary>Sauvegarde les changements en attente dans le contexte EF Core.</summary>
    protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>Recherche par clé primaire (utilise le cache EF Core si disponible).</summary>
    public virtual async Task<TModel?> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull
    {
        return await _context.FindAsync<TModel>(new object[] { id }, cancellationToken);
    }

    /// <summary>Retourne toutes les lignes de la table.</summary>
    public virtual async Task<List<TModel>> ListAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TModel>().ToListAsync(cancellationToken);
    }

    /// <summary>Retourne les lignes correspondant au prédicat LINQ.</summary>
    public virtual async Task<List<TModel>> ListAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TModel>().Where(predicate).ToListAsync(cancellationToken);
    }

    /// <summary>Vérifie l'existence d'au moins une ligne correspondant au prédicat.</summary>
    public virtual async Task<bool> AnyAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TModel>().AnyAsync(predicate, cancellationToken);
    }

    /// <summary>Compte les lignes correspondant au prédicat.</summary>
    public virtual async Task<int> CountAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TModel>().Where(predicate).CountAsync(cancellationToken);
    }

    /// <summary>Compte toutes les lignes de la table.</summary>
    public virtual async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Set<TModel>().CountAsync(cancellationToken);
    }

    /// <summary>Retourne la première ligne correspondant au prédicat, ou null.</summary>
    public virtual async Task<TModel?> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Set<TModel>().FirstOrDefaultAsync(predicate, cancellationToken);
    }
}
