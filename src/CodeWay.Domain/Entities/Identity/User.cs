namespace CodeWay.Domain.Entities.Identity;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Commerce;
using CodeWay.Domain.Entities.Instructor;
using CodeWay.Domain.Entities.Learning;
using CodeWay.Domain.Entities.Notifications;
using CodeWay.Domain.Entities.Payments;

public class User : AuditableEntity, ISoftDelete
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public bool IsActive { get; set; } = true;

    // ISoftDelete implementation
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }
    public Guid? DeletedBy { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    public InstructorProfile? InstructorProfile { get; set; }
    public ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();
    public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    public ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
    public ICollection<Review> Reviews { get; set; } = new HashSet<Review>();
    public Cart? Cart { get; set; }
    public Wishlist? Wishlist { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
}
