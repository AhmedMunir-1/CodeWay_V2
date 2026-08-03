namespace CodeWay.Domain.Entities.Commerce;

using CodeWay.Domain.Common;
using CodeWay.Domain.Entities.Catalog;

public class OrderItem : BaseEntity
{
    public Guid OrderId { get; set; }
    public Guid CourseId { get; set; }
    public decimal UnitPrice { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
