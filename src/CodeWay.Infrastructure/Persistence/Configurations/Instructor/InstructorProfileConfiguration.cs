namespace CodeWay.Infrastructure.Persistence.Configurations.Instructor;

using CodeWay.Domain.Entities.Instructor;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class InstructorProfileConfiguration : IEntityTypeConfiguration<InstructorProfile>
{
    public void Configure(EntityTypeBuilder<InstructorProfile> builder)
    {
        builder.ToTable("InstructorProfiles");

        builder.HasKey(ip => ip.Id);

        builder.HasIndex(ip => ip.UserId)
            .IsUnique();

        builder.Property(ip => ip.Headline)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(ip => ip.Biography)
            .HasMaxLength(4000)
            .IsRequired();

        builder.Property(ip => ip.PayoutEmail)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(ip => ip.WebsiteUrl).HasMaxLength(2048);
        builder.Property(ip => ip.TwitterUrl).HasMaxLength(2048);
        builder.Property(ip => ip.LinkedInUrl).HasMaxLength(2048);
        builder.Property(ip => ip.YouTubeUrl).HasMaxLength(2048);

        builder.HasOne(ip => ip.User)
            .WithOne(u => u.InstructorProfile)
            .HasForeignKey<InstructorProfile>(ip => ip.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
