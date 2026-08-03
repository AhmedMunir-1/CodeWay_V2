namespace CodeWay.Domain.Entities.Commerce;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Entities.Identity;
using CodeWay.Domain.Entities.Payments;

public class Order : AuditableEntity
{
    public Guid UserId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string? CouponCode { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();
    public Payment? Payment { get; set; }
}
