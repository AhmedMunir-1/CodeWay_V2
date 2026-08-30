namespace CodeWay.Infrastructure.Persistence;

using System.Collections.Concurrent;
using CodeWay.Domain.Common;
using CodeWay.Domain.Interfaces;
using CodeWay.Domain.Interfaces.Repositories;
using CodeWay.Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;
    private readonly ConcurrentDictionary<Type, object> _repositories = new();

    private IUserRepository? _users;
    private ICourseRepository? _courses;
    private IEnrollmentRepository? _enrollments;
    private IOrderRepository? _orders;
    private IPaymentRepository? _payments;
    private IInstructorProfileRepository? _instructorProfiles;
    private IWalletRepository? _wallets;
    private IPayoutRequestRepository? _payoutRequests;
    private IReviewRepository? _reviews;
    private INotificationRepository? _notifications;
    private ICouponRepository? _coupons;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public ICourseRepository Courses => _courses ??= new CourseRepository(_context);
    public IEnrollmentRepository Enrollments => _enrollments ??= new EnrollmentRepository(_context);
    public IOrderRepository Orders => _orders ??= new OrderRepository(_context);
    public IPaymentRepository Payments => _payments ??= new PaymentRepository(_context);
    public IInstructorProfileRepository InstructorProfiles => _instructorProfiles ??= new InstructorProfileRepository(_context);
    public IWalletRepository Wallets => _wallets ??= new WalletRepository(_context);
    public IPayoutRequestRepository PayoutRequests => _payoutRequests ??= new PayoutRequestRepository(_context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);
    public INotificationRepository Notifications => _notifications ??= new NotificationRepository(_context);
    public ICouponRepository Coupons => _coupons ??= new CouponRepository(_context);

    public IRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
    {
        return (IRepository<TEntity>)_repositories.GetOrAdd(
            typeof(TEntity),
            _ => new Repository<TEntity>(_context));
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            return;
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
        GC.SuppressFinalize(this);
    }
}
