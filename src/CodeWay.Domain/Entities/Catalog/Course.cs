namespace CodeWay.Domain.Entities.Catalog;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Entities.Instructor;
using CodeWay.Domain.Entities.Learning;

public class Course : AuditableEntity, ISoftDelete
{
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? SubTitle { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Language { get; set; } = "English";
    public CourseLevel Level { get; set; } = CourseLevel.AllLevels;
    public CourseStatus Status { get; set; } = CourseStatus.Draft;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? ThumbnailUrl { get; set; }
    public string? TrailerVideoUrl { get; set; }
    public Guid InstructorId { get; set; }
    public Guid CategoryId { get; set; }
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int TotalEnrollments { get; set; }

    // ISoftDelete implementation
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation properties
    public InstructorProfile Instructor { get; set; } = null!;
    public Category Category { get; set; } = null!;
    public ICollection<Section> Sections { get; set; } = new HashSet<Section>();
    public ICollection<CourseRequirement> Requirements { get; set; } = new HashSet<CourseRequirement>();
    public ICollection<CourseLearningOutcome> LearningOutcomes { get; set; } = new HashSet<CourseLearningOutcome>();
    public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
}
