namespace CodeWay.Infrastructure.Persistence.Configurations.Instructor;

using CodeWay.Domain.Entities.Instructor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class InstructorWalletConfiguration : IEntityTypeConfiguration<InstructorWallet>
{
    public void Configure(EntityTypeBuilder<InstructorWallet> builder)
    {
        builder.ToTable("InstructorWallets");

        builder.HasKey(iw => iw.Id);

        builder.HasIndex(iw => iw.InstructorId)
            .IsUnique();

        builder.Property(iw => iw.Balance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0.00m);

        builder.Property(iw => iw.PendingBalance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0.00m);

        builder.Property(iw => iw.TotalEarned)
            .HasPrecision(18, 2)
            .HasDefaultValue(0.00m);

        builder.Property(iw => iw.RowVersion)
            .IsRowVersion();

        builder.HasOne(iw => iw.Instructor)
            .WithOne(ip => ip.Wallet)
            .HasForeignKey<InstructorWallet>(iw => iw.InstructorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascade delete: InstructorWallet -> WalletTransactions
        builder.HasMany(iw => iw.Transactions)
            .WithOne(wt => wt.Wallet)
            .HasForeignKey(wt => wt.WalletId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
