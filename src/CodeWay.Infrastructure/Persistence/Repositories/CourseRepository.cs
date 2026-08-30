namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

public class CourseRepository : Repository<Course>, ICourseRepository
{
    public CourseRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Course?> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Instructor)
            .Include(c => c.Category)
            .Include(c => c.Sections.OrderBy(s => s.DisplayOrder))
                .ThenInclude(s => s.Lessons.OrderBy(l => l.DisplayOrder))
            .Include(c => c.Requirements)
            .Include(c => c.LearningOutcomes)
            .Include(c => c.Reviews)
            .FirstOrDefaultAsync(c => c.Slug == slug, cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetByInstructorAsync(Guid instructorId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Category)
            .Where(c => c.InstructorId == instructorId)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Course>> GetPublishedAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Instructor)
            .Include(c => c.Category)
            .Where(c => c.Status == CourseStatus.Published)
            .OrderByDescending(c => c.CreatedAtUtc)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> IsSlugTakenAsync(string slug, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(c => c.Slug == slug, cancellationToken);
    }
}
