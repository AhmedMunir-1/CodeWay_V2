namespace CodeWay.Infrastructure.Persistence.Configurations.Catalog;

using CodeWay.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LessonConfiguration : IEntityTypeConfiguration<Lesson>
{
    public void Configure(EntityTypeBuilder<Lesson> builder)
    {
        builder.ToTable("Lessons");

        builder.HasKey(l => l.Id);

        builder.Property(l => l.Title)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(l => l.Description)
            .HasMaxLength(1000);

        builder.Property(l => l.ContentUrl)
            .HasMaxLength(2048);

        builder.Property(l => l.ArticleBody)
            .HasColumnType("nvarchar(max)");

        builder.HasOne(l => l.Section)
            .WithMany(s => s.Lessons)
            .HasForeignKey(l => l.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascade Delete: Lesson -> LessonAttachments
        builder.HasMany(l => l.Attachments)
            .WithOne(la => la.Lesson)
            .HasForeignKey(la => la.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
