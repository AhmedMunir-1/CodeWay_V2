namespace CodeWay.Domain.Entities.Catalog;

using CodeWay.Domain.Common;

public class LessonAttachment : BaseEntity
{
    public Guid LessonId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSizeInBytes { get; set; }

    // Navigation properties
    public Lesson Lesson { get; set; } = null!;
}
