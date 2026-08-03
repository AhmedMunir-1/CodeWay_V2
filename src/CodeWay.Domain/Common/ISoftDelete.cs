namespace CodeWay.Domain.Common;

/// <summary>
/// Marks an entity as soft-deletable.
/// The AuditableEntityInterceptor in Infrastructure sets IsDeleted = true
/// and populates DeletedAtUtc instead of issuing a physical DELETE statement.
/// </summary>
public interface ISoftDelete
{
    bool IsDeleted { get; set; }
    DateTime? DeletedAtUtc { get; set; }
    Guid? DeletedBy { get; set; }
}
