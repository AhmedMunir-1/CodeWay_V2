namespace CodeWay.Infrastructure.Persistence.Configurations.Identity;

using CodeWay.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        builder.HasKey(r => r.Id);

        builder.Property(r => r.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(r => r.NormalizedName)
            .HasMaxLength(100)
            .IsRequired();

        builder.HasIndex(r => r.NormalizedName)
            .IsUnique();

        builder.Property(r => r.Description)
            .HasMaxLength(500);
    }
}
