namespace CodeWay.Infrastructure.Persistence.Configurations.Learning;

using CodeWay.Domain.Entities.Learning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LessonProgressConfiguration : IEntityTypeConfiguration<LessonProgress>
{
    public void Configure(EntityTypeBuilder<LessonProgress> builder)
    {
        builder.ToTable("LessonProgresses");

        builder.HasKey(lp => lp.Id);

        builder.HasIndex(lp => new { lp.EnrollmentId, lp.LessonId })
            .IsUnique();

        builder.HasOne(lp => lp.Enrollment)
            .WithMany(e => e.LessonProgresses)
            .HasForeignKey(lp => lp.EnrollmentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(lp => lp.Lesson)
            .WithMany(l => l.LessonProgresses)
            .HasForeignKey(lp => lp.LessonId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
