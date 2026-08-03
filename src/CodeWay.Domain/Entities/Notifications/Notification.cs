namespace CodeWay.Domain.Entities.Notifications;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Entities.Identity;

public class Notification : AuditableEntity
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public NotificationType Type { get; set; } = NotificationType.Info;
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    public string? ActionUrl { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
