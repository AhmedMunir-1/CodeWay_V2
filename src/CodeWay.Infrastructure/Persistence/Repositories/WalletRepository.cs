namespace CodeWay.Infrastructure.Persistence.Repositories;

using CodeWay.Domain.Entities.Instructor;
using CodeWay.Domain.Interfaces.Repositories;

public class WalletRepository : Repository<InstructorWallet>, IWalletRepository
{
    public WalletRepository(ApplicationDbContext context) : base(context)
    {
    }
}
