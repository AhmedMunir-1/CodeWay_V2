namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Learning;
using CodeWay.Domain.Interfaces.Repositories;

public class EnrollmentRepository : Repository<Enrollment>, IEnrollmentRepository
{
    public EnrollmentRepository(ApplicationDbContext context) : base(context)
    {
    }
}
