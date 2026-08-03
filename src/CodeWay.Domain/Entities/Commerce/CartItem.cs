namespace CodeWay.Domain.Entities.Commerce;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Catalog;

public class CartItem : BaseEntity
{
    public Guid CartId { get; set; }
    public Guid CourseId { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation properties
    public Cart Cart { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
