namespace CodeWay.Infrastructure.Persistence.Configurations.Instructor;

using CodeWay.Domain.Entities.Instructor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> builder)
    {
        builder.ToTable("WalletTransactions");

        builder.HasKey(wt => wt.Id);

        builder.Property(wt => wt.Amount)
            .HasPrecision(18, 2);

        builder.Property(wt => wt.Description)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(wt => wt.ReferenceId)
            .HasMaxLength(100);

        builder.HasOne(wt => wt.Wallet)
            .WithMany(w => w.Transactions)
            .HasForeignKey(wt => wt.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
