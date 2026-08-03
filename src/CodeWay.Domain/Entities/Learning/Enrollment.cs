namespace CodeWay.Domain.Entities.Learning;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Entities.Identity;

public class Enrollment : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;
    public double ProgressPercentage { get; set; }
    public DateTime EnrolledAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? CompletedAtUtc { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
    public ICollection<LessonProgress> LessonProgresses { get; set; } = new HashSet<LessonProgress>();
    public Certificate? Certificate { get; set; }
}
