namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Instructor;
using CodeWay.Domain.Interfaces.Repositories;

public class InstructorProfileRepository : Repository<InstructorProfile>, IInstructorProfileRepository
{
    public InstructorProfileRepository(ApplicationDbContext context) : base(context)
    {
    }
}
