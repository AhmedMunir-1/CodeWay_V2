namespace CodeWay.Domain.Entities.Identity;

using CodeWay.Domain.Common;

public class Role : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string NormalizedName { get; set; } = string.Empty;
    public string? Description { get; set; }

    // Navigation properties
    public ICollection<UserRole> UserRoles { get; set; } = new HashSet<UserRole>();
}
