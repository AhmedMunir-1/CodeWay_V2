namespace CodeWay.Infrastructure.Persistence.Configurations.Payments;

using CodeWay.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.OrderId)
            .IsUnique();

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2);

        builder.Property(p => p.Currency)
            .HasMaxLength(10)
            .HasDefaultValue("USD");

        builder.Property(p => p.PaymentMethod)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(p => p.TransactionId)
            .HasMaxLength(255);

        builder.Property(p => p.FailureReason)
            .HasMaxLength(1000);

        // Restrict Delete: User -> Payments
        builder.HasOne(p => p.User)
            .WithMany(u => u.Payments)
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict Delete: Order -> Payments
        builder.HasOne(p => p.Order)
            .WithOne(o => o.Payment)
            .HasForeignKey<Payment>(p => p.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
