namespace CodeWay.Domain.Entities.Learning;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Entities.Identity;

public class Review : AuditableEntity
{
    public Guid UserId { get; set; }
    public Guid CourseId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
