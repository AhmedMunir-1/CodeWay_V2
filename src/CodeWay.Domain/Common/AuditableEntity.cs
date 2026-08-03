namespace CodeWay.Domain.Common;

/// <summary>
/// Extends <see cref="BaseEntity"/> with audit metadata.
/// The <c>AuditableEntityInterceptor</c> in Infrastructure automatically
/// populates these fields before every SaveChanges call.
/// </summary>
public abstract class AuditableEntity : BaseEntity
{
    public DateTime CreatedAtUtc { get; set; }
    public DateTime UpdatedAtUtc { get; set; }

    /// <summary>UserId of the user who created this record.</summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>UserId of the user who last modified this record.</summary>
    public Guid? UpdatedBy { get; set; }
}
