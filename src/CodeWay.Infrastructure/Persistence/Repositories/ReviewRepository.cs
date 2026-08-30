namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Learning;
using CodeWay.Domain.Interfaces.Repositories;

public class ReviewRepository : Repository<Review>, IReviewRepository
{
    public ReviewRepository(ApplicationDbContext context) : base(context)
    {
    }
}
