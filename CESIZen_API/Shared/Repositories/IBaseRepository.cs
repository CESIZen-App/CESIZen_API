using System.Linq.Expressions;

namespace CESIZen_API.Shared.Repositories;

public interface IBaseRepository<TModel> where TModel : class
{
    Task<TModel> AddAsync(TModel model, CancellationToken cancellationToken = default);
    Task UpdateAsync(TModel model, CancellationToken cancellationToken = default);
    Task DeleteAsync(TModel model, CancellationToken cancellationToken = default);
    Task<TModel?> FindAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
    Task<List<TModel>> ListAsync(CancellationToken cancellationToken = default);
    Task<List<TModel>> ListAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
    Task<bool> AnyAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
    Task<int> CountAsync(CancellationToken cancellationToken = default);
    Task<TModel?> FirstOrDefaultAsync(Expression<Func<TModel, bool>> predicate, CancellationToken cancellationToken = default);
}