namespace CodeWay.Application.Features.Commerce.DTOs;

using CodeWay.Domain.Enums;

public class CartDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public decimal TotalAmount { get; set; }
    public List<CartItemDto> Items { get; set; } = [];
}

public class CartItemDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CourseSlug { get; set; } = string.Empty;
    public string? CourseThumbnailUrl { get; set; }
    public decimal UnitPrice { get; set; }
}

public class AddToCartDto
{
    public Guid CourseId { get; set; }
}

public class WishlistDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public List<WishlistItemDto> Items { get; set; } = [];
}

public class WishlistItemDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public string CourseSlug { get; set; } = string.Empty;
    public string? CourseThumbnailUrl { get; set; }
    public decimal Price { get; set; }
    public DateTime AddedAtUtc { get; set; }
}

public class AddToWishlistDto
{
    public Guid CourseId { get; set; }
}

public class OrderDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal FinalAmount { get; set; }
    public OrderStatus Status { get; set; }
    public string? CouponCode { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<OrderItemDto> Items { get; set; } = [];
}

public class OrderItemDto
{
    public Guid Id { get; set; }
    public Guid CourseId { get; set; }
    public string CourseTitle { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}

public class CheckoutDto
{
    public string? CouponCode { get; set; }
    public string PaymentMethod { get; set; } = "CreditCard";
}

public class CouponDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public int MaxUses { get; set; }
    public int TimesUsed { get; set; }
    public DateTime ValidFromUtc { get; set; }
    public DateTime ValidUntilUtc { get; set; }
    public bool IsActive { get; set; }
}

public class CreateCouponDto
{
    public string Code { get; set; } = string.Empty;
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public int MaxUses { get; set; }
    public DateTime ValidFromUtc { get; set; }
    public DateTime ValidUntilUtc { get; set; }
    public bool IsActive { get; set; } = true;
}

public class UpdateCouponDto
{
    public DiscountType DiscountType { get; set; }
    public decimal DiscountValue { get; set; }
    public int MaxUses { get; set; }
    public DateTime ValidFromUtc { get; set; }
    public DateTime ValidUntilUtc { get; set; }
    public bool IsActive { get; set; }
}

public class CouponValidationResultDto
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public DiscountType? DiscountType { get; set; }
    public decimal? DiscountValue { get; set; }
    public decimal DiscountCalculated { get; set; }
}
