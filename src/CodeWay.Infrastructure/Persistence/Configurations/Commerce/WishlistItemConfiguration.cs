namespace CodeWay.Infrastructure.Persistence.Configurations.Commerce;

using CodeWay.Domain.Entities.Commerce;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class WishlistItemConfiguration : IEntityTypeConfiguration<WishlistItem>
{
    public void Configure(EntityTypeBuilder<WishlistItem> builder)
    {
        builder.ToTable("WishlistItems");

        builder.HasKey(wi => wi.Id);

        builder.HasIndex(wi => new { wi.WishlistId, wi.CourseId })
            .IsUnique();

        builder.HasOne(wi => wi.Wishlist)
            .WithMany(w => w.Items)
            .HasForeignKey(wi => wi.WishlistId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(wi => wi.Course)
            .WithMany()
            .HasForeignKey(wi => wi.CourseId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
