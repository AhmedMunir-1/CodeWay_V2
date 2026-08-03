namespace CodeWay.Domain.Entities.Instructor;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;

public class PayoutRequest : AuditableEntity
{
    public Guid InstructorId { get; set; }
    public decimal Amount { get; set; }
    public PayoutStatus Status { get; set; } = PayoutStatus.Pending;
    public string PayoutMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }

    // Navigation properties
    public InstructorProfile Instructor { get; set; } = null!;
}
