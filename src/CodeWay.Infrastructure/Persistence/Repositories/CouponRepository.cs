namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Commerce;
using CodeWay.Domain.Interfaces.Repositories;

public class CouponRepository : Repository<Coupon>, ICouponRepository
{
    public CouponRepository(ApplicationDbContext context) : base(context)
    {
    }
}
