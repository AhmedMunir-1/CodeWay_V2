namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Commerce;
using CodeWay.Domain.Interfaces.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context)
    {
    }
}
