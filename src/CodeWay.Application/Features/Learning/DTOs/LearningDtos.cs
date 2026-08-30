namespace CodeWay.Application.Features.Learning.DTOs;

using CodeWay.Domain.Enums;

public class EnrollmentDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CourseSlug { get; set; } = string.Empty;
    public string? CourseThumbnailUrl { get; set; }
    public EnrollmentStatus Status { get; set; }
    public double ProgressPercentage { get; set; }
    public DateTime EnrolledAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public CertificateDto? Certificate { get; set; }
    public List<LessonProgressDto> LessonProgresses { get; set; } = [];
}

public class CreateEnrollmentDto
{
    public Guid CourseId { get; set; }
}

public class LessonProgressDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public Guid LessonId { get; set; }
    public string LessonTitle { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    public int LastWatchedPositionInSeconds { get; set; }
}

public class UpdateLessonProgressDto
{
    public Guid EnrollmentId { get; set; }
    public Guid LessonId { get; set; }
    public bool IsCompleted { get; set; }
    public int LastWatchedPositionInSeconds { get; set; }
}

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserProfilePictureUrl { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreateReviewDto
{
    public Guid CourseId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class UpdateReviewDto
{
    public int Rating { get; set; }
    public string? Comment { get; set; }
}

public class CertificateDto
{
    public Guid Id { get; set; }
    public Guid EnrollmentId { get; set; }
    public string CertificateCode { get; set; } = string.Empty;
    public DateTime IssuedAtUtc { get; set; }
    public string PdfUrl { get; set; } = string.Empty;
}
