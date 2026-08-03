namespace CodeWay.Infrastructure.Persistence.Configurations.Catalog;

using CodeWay.Domain.Entities.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class LessonAttachmentConfiguration : IEntityTypeConfiguration<LessonAttachment>
{
    public void Configure(EntityTypeBuilder<LessonAttachment> builder)
    {
        builder.ToTable("LessonAttachments");

        builder.HasKey(la => la.Id);

        builder.Property(la => la.FileName)
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(la => la.FileUrl)
            .HasMaxLength(2048)
            .IsRequired();

        builder.HasOne(la => la.Lesson)
            .WithMany(l => l.Attachments)
            .HasForeignKey(la => la.LessonId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
