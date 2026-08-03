namespace CodeWay.Domain.Entities.Commerce;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Catalog;

public class WishlistItem : BaseEntity
{
    public Guid WishlistId { get; set; }
    public Guid CourseId { get; set; }
    public DateTime AddedAtUtc { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public Wishlist Wishlist { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
