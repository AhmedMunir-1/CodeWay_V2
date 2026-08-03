namespace CodeWay.Infrastructure.Persistence.Configurations.Instructor;

using CodeWay.Domain.Entities.Instructor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class PayoutRequestConfiguration : IEntityTypeConfiguration<PayoutRequest>
{
    public void Configure(EntityTypeBuilder<PayoutRequest> builder)
    {
        builder.ToTable("PayoutRequests");

        builder.HasKey(pr => pr.Id);

        builder.Property(pr => pr.Amount)
            .HasPrecision(18, 2);

        builder.Property(pr => pr.PayoutMethod)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(pr => pr.Notes)
            .HasMaxLength(1000);

        builder.HasOne(pr => pr.Instructor)
            .WithMany(ip => ip.PayoutRequests)
            .HasForeignKey(pr => pr.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
