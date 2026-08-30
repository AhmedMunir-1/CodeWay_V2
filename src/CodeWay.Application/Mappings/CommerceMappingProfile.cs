namespace CodeWay.Application.Mappings;

using AutoMapper;
using CodeWay.Application.Features.Commerce.DTOs;
using CodeWay.Domain.Entities.Commerce;

public class CommerceMappingProfile : Profile
{
    public CommerceMappingProfile()
    {
        CreateMap<Cart, CartDto>();
        CreateMap<CartItem, CartItemDto>()
            .ForMember(d => d.CourseTitle, opt => opt.MapFrom(s => s.Course != null ? s.Course.Title : string.Empty))
            .ForMember(d => d.CourseSlug, opt => opt.MapFrom(s => s.Course != null ? s.Course.Slug : string.Empty))
            .ForMember(d => d.CourseThumbnailUrl, opt => opt.MapFrom(s => s.Course != null ? s.Course.ThumbnailUrl : null));

        CreateMap<Wishlist, WishlistDto>();
        CreateMap<WishlistItem, WishlistItemDto>()
            .ForMember(d => d.CourseTitle, opt => opt.MapFrom(s => s.Course != null ? s.Course.Title : string.Empty))
            .ForMember(d => d.CourseSlug, opt => opt.MapFrom(s => s.Course != null ? s.Course.Slug : string.Empty))
            .ForMember(d => d.CourseThumbnailUrl, opt => opt.MapFrom(s => s.Course != null ? s.Course.ThumbnailUrl : null))
            .ForMember(d => d.Price, opt => opt.MapFrom(s => s.Course != null ? (s.Course.DiscountPrice ?? s.Course.Price) : 0));

        CreateMap<Order, OrderDto>()
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User != null ? $"{s.User.FirstName} {s.User.LastName}" : string.Empty));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.CourseTitle, opt => opt.MapFrom(s => s.Course != null ? s.Course.Title : string.Empty));

        CreateMap<Coupon, CouponDto>();
    }
}
