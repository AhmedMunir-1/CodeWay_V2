namespace CodeWay.Domain.Entities.Identity;

using CodeWay.Domain.Common;

public class RefreshToken : BaseEntity
{
    public Guid UserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAtUtc { get; set; }
    public string CreatedByIp { get; set; } = string.Empty;
    public DateTime? RevokedAtUtc { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
}
