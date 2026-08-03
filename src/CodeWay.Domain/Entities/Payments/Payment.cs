namespace CodeWay.Domain.Entities.Payments;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Entities.Commerce;
using CodeWay.Domain.Entities.Identity;

public class Payment : AuditableEntity
{
    public Guid OrderId { get; set; }
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public PaymentStatus Status { get; set; } = PaymentStatus.Pending;
    public string PaymentMethod { get; set; } = string.Empty;
    public string? TransactionId { get; set; }
    public string? FailureReason { get; set; }
    public DateTime? PaidAtUtc { get; set; }

    // Navigation properties
    public Order Order { get; set; } = null!;
    public User User { get; set; } = null!;
}
