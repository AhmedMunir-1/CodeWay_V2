namespace CodeWay.Domain.Entities.Catalog;

using CodeWay.Domain.Common;

public class CourseLearningOutcome : BaseEntity
{
    public Guid CourseId { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }

    // Navigation properties
    public Course Course { get; set; } = null!;
}
