namespace CodeWay.Domain.Interfaces.Repositories;

using CodeWay.Domain.Entities.Identity;

/// <summary>User-specific repository — extends generic CRUD with identity queries.</summary>
public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken = default);
    Task<User?> GetWithRolesAsync(Guid userId, CancellationToken cancellationToken = default);
}
