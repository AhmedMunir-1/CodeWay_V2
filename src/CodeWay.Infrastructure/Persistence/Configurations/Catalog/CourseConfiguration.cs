namespace CodeWay.Infrastructure.Persistence.Configurations.Catalog;

using CodeWay.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
    public void Configure(EntityTypeBuilder<Course> builder)
    {
        builder.ToTable("Courses");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.Slug)
            .HasMaxLength(250)
            .IsRequired();

        builder.HasIndex(c => c.Slug)
            .IsUnique();

        builder.Property(c => c.SubTitle)
            .HasMaxLength(300);

        builder.Property(c => c.Description)
            .HasMaxLength(10000)
            .IsRequired();

        builder.Property(c => c.Language)
            .HasMaxLength(50)
            .HasDefaultValue("English");

        builder.Property(c => c.Price)
            .HasPrecision(18, 2);

        builder.Property(c => c.DiscountPrice)
            .HasPrecision(18, 2);

        builder.Property(c => c.ThumbnailUrl)
            .HasMaxLength(2048);

        builder.Property(c => c.TrailerVideoUrl)
            .HasMaxLength(2048);

        // Soft delete query filter
        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasOne(c => c.Instructor)
            .WithMany(i => i.Courses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(c => c.Category)
            .WithMany(cat => cat.Courses)
            .HasForeignKey(c => c.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Cascade Delete: Course -> Sections
        builder.HasMany(c => c.Sections)
            .WithOne(s => s.Course)
            .HasForeignKey(s => s.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.Requirements)
            .WithOne(cr => cr.Course)
            .HasForeignKey(cr => cr.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(c => c.LearningOutcomes)
            .WithOne(clo => clo.Course)
            .HasForeignKey(clo => clo.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
