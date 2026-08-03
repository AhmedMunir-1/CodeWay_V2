namespace CodeWay.Infrastructure.Persistence.Configurations.Learning;

using CodeWay.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CertificateConfiguration : IEntityTypeConfiguration<Certificate>
{
    public void Configure(EntityTypeBuilder<Certificate> builder)
    {
        builder.ToTable("Certificates");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.CertificateCode)
            .HasMaxLength(50)
            .IsRequired();

        builder.HasIndex(c => c.CertificateCode)
            .IsUnique();

        builder.Property(c => c.PdfUrl)
            .HasMaxLength(2048)
            .IsRequired();

        builder.HasOne(c => c.Enrollment)
            .WithOne(e => e.Certificate)
            .HasForeignKey<Certificate>(c => c.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
