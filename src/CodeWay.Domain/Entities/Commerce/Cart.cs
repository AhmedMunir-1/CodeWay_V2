namespace CodeWay.Domain.Entities.Commerce;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Identity;

public class Cart : AuditableEntity
{
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<CartItem> Items { get; set; } = new HashSet<CartItem>();
}
