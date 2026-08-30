namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Payments;
using CodeWay.Domain.Interfaces.Repositories;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(ApplicationDbContext context) : base(context)
    {
    }
}
