namespace CodeWay.Domain.Entities.Catalog;

using CodeWay.Domain.Common;

public class CourseRequirement : BaseEntity
{
    public Guid CourseId { get; set; }
    public string Requirement { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;
}
