namespace CodeWay.Domain.Entities.Learning;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Catalog;

public class LessonProgress : AuditableEntity
{
    public Guid EnrollmentId { get; set; }
    public Guid LessonId { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public int LastWatchedPositionInSeconds { get; set; }

    // Navigation properties
    public Enrollment Enrollment { get; set; } = null!;
    public Lesson Lesson { get; set; } = null!;
}
