namespace CodeWay.Domain.Interfaces.Repositories;

using CodeWay.Domain.Entities.Catalog;

/// <summary>Course-specific repository — extends generic CRUD with catalog queries.</summary>
public interface ICourseRepository : IRepository<Course>
{
    Task<Course?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Course>> GetPublishedAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<bool> IsSlugTakenAsync(string slug, CancellationToken cancellationToken = default);
}
