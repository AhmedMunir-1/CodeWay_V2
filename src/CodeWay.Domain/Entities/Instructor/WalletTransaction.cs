namespace CodeWay.Domain.Entities.Instructor;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;

public class WalletTransaction : AuditableEntity
{
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public WalletTransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }

    // Navigation properties
    public InstructorWallet Wallet { get; set; } = null!;
}
