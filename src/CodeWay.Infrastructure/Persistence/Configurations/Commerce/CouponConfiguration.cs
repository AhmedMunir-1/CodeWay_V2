namespace CodeWay.Infrastructure.Persistence.Configurations.Commerce;

using CodeWay.Domain.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
{
    public void Configure(EntityTypeBuilder<Coupon> builder)
    {
        builder.ToTable("Coupons");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Code)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => c.Code)
            .IsUnique();

        builder.Property(c => c.DiscountValue)
            .HasPrecision(18, 2);

        builder.Property(c => c.IsActive)
            .HasDefaultValue(true);
    }
}
