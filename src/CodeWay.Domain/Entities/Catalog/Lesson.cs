namespace CodeWay.Domain.Entities.Catalog;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Entities.Learning;

public class Lesson : AuditableEntity
{
    public Guid SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public int DurationInSeconds { get; set; }
    public LessonType Type { get; set; } = LessonType.Video;
    public string? ContentUrl { get; set; }
    public string? ArticleBody { get; set; }
    public bool IsFreePreview { get; set; }

    // Navigation properties
    public Section Section { get; set; } = null!;
    public ICollection<LessonAttachment> Attachments { get; set; } = new HashSet<LessonAttachment>();
    public ICollection<LessonProgress> LessonProgresses { get; set; } = new HashSet<LessonProgress>();
}
