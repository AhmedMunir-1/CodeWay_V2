namespace CodeWay.Domain.Entities.Commerce;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Identity;

public class Wishlist : AuditableEntity
{
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<WishlistItem> Items { get; set; } = new HashSet<WishlistItem>();
}
