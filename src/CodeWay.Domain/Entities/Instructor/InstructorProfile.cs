namespace CodeWay.Domain.Entities.Instructor;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Entities.Identity;

public class InstructorProfile : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Headline { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public string PayoutEmail { get; set; } = string.Empty;
    public bool IsApproved { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public InstructorWallet? Wallet { get; set; }
    public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
    public ICollection<PayoutRequest> PayoutRequests { get; set; } = new HashSet<PayoutRequest>();
}
