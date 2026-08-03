namespace CodeWay.Domain.Entities.Instructor;

using CodeWay.Domain.Common;

public class InstructorWallet : AuditableEntity
{
    public Guid InstructorId { get; set; }
    public decimal Balance { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal TotalEarned { get; set; }
    public byte[] RowVersion { get; set; } = [];

    // Navigation properties
    public InstructorProfile Instructor { get; set; } = null!;
    public ICollection<WalletTransaction> Transactions { get; set; } = new HashSet<WalletTransaction>();
}
