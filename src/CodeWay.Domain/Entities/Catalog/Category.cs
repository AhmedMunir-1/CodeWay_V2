namespace CodeWay.Domain.Entities.Catalog;

using CodeWay.Domain.Common;

public class Category : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation properties
    public Category? ParentCategory { get; set; }
    public ICollection<Category> SubCategories { get; set; } = new HashSet<Category>();
    public ICollection<Course> Courses { get; set; } = new HashSet<Course>();
}
