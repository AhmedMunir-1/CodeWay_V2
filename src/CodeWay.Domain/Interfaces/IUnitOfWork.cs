namespace CodeWay.Domain.Interfaces;

/// <summary>
/// Unit of Work contract — wraps all repositories under a single transaction.
/// One <see cref="SaveChangesAsync"/> call per HTTP request ensures atomicity.
/// Implemented in Infrastructure by <c>UnitOfWork</c> which holds the <c>ApplicationDbContext</c>.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // ── Repositories (accessed via UoW to share the same DbContext) ──────────

    Repositories.IUserRepository Users { get; }
    Repositories.ICourseRepository Courses { get; }
    Repositories.IEnrollmentRepository Enrollments { get; }
    Repositories.IOrderRepository Orders { get; }
    Repositories.IPaymentRepository Payments { get; }
    Repositories.IInstructorProfileRepository InstructorProfiles { get; }
    Repositories.IWalletRepository Wallets { get; }
    Repositories.IPayoutRequestRepository PayoutRequests { get; }
    Repositories.IReviewRepository Reviews { get; }
    Repositories.INotificationRepository Notifications { get; }
    Repositories.ICouponRepository Coupons { get; }

    // ── Persistence ──────────────────────────────────────────────────────────

    /// <summary>Persist all pending changes to the database.</summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>Begin an explicit database transaction (for complex multi-step operations).</summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Commit the current transaction.</summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>Roll back the current transaction.</summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
