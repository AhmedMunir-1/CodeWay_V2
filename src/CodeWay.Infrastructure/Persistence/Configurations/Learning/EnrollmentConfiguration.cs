namespace CodeWay.Infrastructure.Persistence.Configurations.Learning;

using CodeWay.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
    public void Configure(EntityTypeBuilder<Enrollment> builder)
    {
        builder.ToTable("Enrollments");

        builder.HasKey(e => e.Id);

        builder.HasIndex(e => new { e.UserId, e.CourseId })
            .IsUnique();

        // Restrict Delete: User -> Enrollments
        builder.HasOne(e => e.User)
            .WithMany(u => u.Enrollments)
            .HasForeignKey(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(e => e.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(e => e.CourseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.LessonProgresses)
            .WithOne(lp => lp.Enrollment)
            .HasForeignKey(lp => lp.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.Certificate)
            .WithOne(cert => cert.Enrollment)
            .HasForeignKey<Certificate>(cert => cert.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
