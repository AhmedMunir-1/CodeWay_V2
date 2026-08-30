namespace CodeWay.Application.Features.Catalog.DTOs;

using CodeWay.Domain.Enums;

public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SubTitle { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "English";
    public CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TrailerVideoUrl { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalEnrollments { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CourseDetailDto : CourseDto
{
    public List<SectionDto> Sections { get; set; } = [];
    public List<CourseRequirementDto> Requirements { get; set; } = [];
    public List<CourseLearningOutcomeDto> LearningOutcomes { get; set; } = [];
}

public class CreateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string? SubTitle { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "English";
    public CourseLevel Level { get; set; } = CourseLevel.AllLevels;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TrailerVideoUrl { get; set; }
    public Guid InstructorId { get; set; }
    public Guid CategoryId { get; set; }
    public List<string>? Requirements { get; set; }
    public List<string>? LearningOutcomes { get; set; }
}

public class UpdateCourseDto
{
    public string Title { get; set; } = string.Empty;
    public string? SubTitle { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "English";
    public CourseLevel Level { get; set; }
    public CourseStatus Status { get; set; }
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TrailerVideoUrl { get; set; }
    public Guid CategoryId { get; set; }
}

public class SectionDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public List<LessonDto> Lessons { get; set; } = [];
}

public class CreateSectionDto
{
    public Guid CourseId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}

public class UpdateSectionDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
}

public class LessonDto
{
    public Guid Id { get; set; }
    public Guid SectionId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public int DurationInSeconds { get; set; }
    public LessonType Type { get; set; }
    public string? ContentUrl { get; set; }
    public string? ArticleBody { get; set; }
    public bool IsFreePreview { get; set; }
    public List<LessonAttachmentDto> Attachments { get; set; } = [];
}

public class CreateLessonDto
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
}

public class UpdateLessonDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public int DurationInSeconds { get; set; }
    public LessonType Type { get; set; }
    public string? ContentUrl { get; set; }
    public string? ArticleBody { get; set; }
    public bool IsFreePreview { get; set; }
}

public class LessonAttachmentDto
{
    public Guid Id { get; set; }
    public Guid LessonId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public string FileType { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class CourseRequirementDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Requirement { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}

public class CourseLearningOutcomeDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string Outcome { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
}
