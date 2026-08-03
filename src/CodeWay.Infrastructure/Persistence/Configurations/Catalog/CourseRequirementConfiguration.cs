namespace CodeWay.Infrastructure.Persistence.Configurations.Catalog;

using CodeWay.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CourseRequirementConfiguration : IEntityTypeConfiguration<CourseRequirement>
{
    public void Configure(EntityTypeBuilder<CourseRequirement> builder)
    {
        builder.ToTable("CourseRequirements");

        builder.HasKey(cr => cr.Id);

        builder.Property(cr => cr.Requirement)
            .HasMaxLength(500)
            .IsRequired();

        builder.HasOne(cr => cr.Course)
            .WithMany(c => c.Requirements)
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
