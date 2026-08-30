namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Instructor;
using CodeWay.Domain.Interfaces.Repositories;

public class PayoutRequestRepository : Repository<PayoutRequest>, IPayoutRequestRepository
{
    public PayoutRequestRepository(ApplicationDbContext context) : base(context)
    {
    }
}
