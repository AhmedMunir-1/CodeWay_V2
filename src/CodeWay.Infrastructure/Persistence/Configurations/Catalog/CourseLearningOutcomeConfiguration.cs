namespace CodeWay.Infrastructure.Persistence.Configurations.Catalog;

using CodeWay.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CourseLearningOutcomeConfiguration : IEntityTypeConfiguration<CourseLearningOutcome>
{
    public void Configure(EntityTypeBuilder<CourseLearningOutcome> builder)
    {
        builder.ToTable("CourseLearningOutcomes");

        builder.HasKey(clo => clo.Id);

        builder.Property(clo => clo.Outcome)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(clo => clo.Course)
            .WithMany(c => c.LearningOutcomes)
            .HasForeignKey(clo => clo.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
