namespace CodeWay.Domain.Interfaces;

using System.Linq.Expressions;
using CodeWay.Domain.Common;
using CodeWay.Domain.Specifications;

/// <summary>
/// Generic repository contract — defines the CRUD surface that all concrete
/// repositories must implement. Lives in Domain to enforce Dependency Inversion:
/// Infrastructure depends on Domain, not the reverse.
/// </summary>
/// <typeparam name="T">An entity that inherits from <see cref="BaseEntity"/>.</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    // ── Queries ──────────────────────────────────────────────────────────────

    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    Task<T?> GetFirstOrDefaultAsync(
        ISpecification<T> specification,
        CancellationToken cancellationToken = default);

    Task<bool> ExistsAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);

    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    // ── Commands ─────────────────────────────────────────────────────────────

    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
    void RemoveRange(IEnumerable<T> entities);
}
