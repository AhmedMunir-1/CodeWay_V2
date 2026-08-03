namespace CodeWay.Domain.Entities.Commerce;

using CodeWay.Domain.Common;
using CodeWay.Domain.Enums;

public class Coupon : AuditableEntity
{
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public int MaxUses { get; set; }
    public int TimesUsed { get; set; }
    public DateTime ValidFromUtc { get; set; }
    public DateTime ValidUntilUtc { get; set; }
    public bool IsActive { get; set; } = true;
}
