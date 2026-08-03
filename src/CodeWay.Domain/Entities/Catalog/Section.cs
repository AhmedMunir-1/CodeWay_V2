namespace CodeWay.Domain.Entities.Catalog;

using CodeWay.Domain.Common;

public class Section : AuditableEntity
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;
    public ICollection<Lesson> Lessons { get; set; } = new HashSet<Lesson>();
}
