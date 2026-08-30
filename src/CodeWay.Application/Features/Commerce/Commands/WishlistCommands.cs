namespace CodeWay.Application.Features.Commerce.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Commerce.DTOs;
using CodeWay.Domain.Entities.Commerce;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record GetWishlistQuery : IRequest<WishlistDto>;

public sealed record AddToWishlistCommand(AddToWishlistDto Dto) : IRequest<WishlistDto>;

public sealed record RemoveFromWishlistCommand(Guid CourseId) : IRequest<WishlistDto>;

public sealed class WishlistCommandHandler :
    IRequestHandler<GetWishlistQuery, WishlistDto>,
    IRequestHandler<AddToWishlistCommand, WishlistDto>,
    IRequestHandler<RemoveFromWishlistCommand, WishlistDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public WishlistCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<WishlistDto> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required to access wishlist.");

        var wishlist = await GetOrCreateWishlistAsync(userId, cancellationToken);
        return MapWishlistDto(wishlist);
    }

    public async Task<WishlistDto> Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required.");

        var course = await _unitOfWork.Courses.GetByIdAsync(request.Dto.CourseId, cancellationToken)
            ?? throw new NotFoundException("Course", request.Dto.CourseId);

        var wishlist = await GetOrCreateWishlistAsync(userId, cancellationToken);

        if (wishlist.Items.Any(i => i.CourseId == request.Dto.CourseId))
            return MapWishlistDto(wishlist); // Idempotent

        var item = new WishlistItem
        {
            WishlistId = wishlist.Id,
            CourseId = course.Id,
            AddedAtUtc = DateTime.UtcNow,
            Course = course
        };

        var wishlistItemRepo = _unitOfWork.Repository<WishlistItem>();
        await wishlistItemRepo.AddAsync(item, cancellationToken);
        wishlist.Items.Add(item);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapWishlistDto(wishlist);
    }

    public async Task<WishlistDto> Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required.");

        var wishlist = await GetOrCreateWishlistAsync(userId, cancellationToken);

        var item = wishlist.Items.FirstOrDefault(i => i.CourseId == request.CourseId);
        if (item != null)
        {
            var wishlistItemRepo = _unitOfWork.Repository<WishlistItem>();
            wishlistItemRepo.Remove(item);
            wishlist.Items.Remove(item);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        return MapWishlistDto(wishlist);
    }

    private async Task<Wishlist> GetOrCreateWishlistAsync(Guid userId, CancellationToken cancellationToken)
    {
        var wishlistRepo = _unitOfWork.Repository<Wishlist>();
        var wishlists = await wishlistRepo.GetAsync(w => w.UserId == userId, cancellationToken);
        var wishlist = wishlists.FirstOrDefault();

        if (wishlist == null)
        {
            wishlist = new Wishlist
            {
                UserId = userId
            };
            await wishlistRepo.AddAsync(wishlist, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var itemRepo = _unitOfWork.Repository<WishlistItem>();
        var items = await itemRepo.GetAsync(i => i.WishlistId == wishlist.Id, cancellationToken);
        var courseRepo = _unitOfWork.Courses;

        foreach (var item in items)
        {
            if (item.Course == null)
            {
                item.Course = (await courseRepo.GetByIdAsync(item.CourseId, cancellationToken))!;
            }
        }

        wishlist.Items = items.ToList();
        return wishlist;
    }

    private static WishlistDto MapWishlistDto(Wishlist wishlist)
    {
        return new WishlistDto
        {
            Id = wishlist.Id,
            UserId = wishlist.UserId,
            Items = wishlist.Items.Select(i => new WishlistItemDto
            {
                Id = i.Id,
                CourseId = i.CourseId,
                CourseTitle = i.Course?.Title ?? string.Empty,
                CourseSlug = i.Course?.Slug ?? string.Empty,
                CourseThumbnailUrl = i.Course?.ThumbnailUrl,
                Price = i.Course?.DiscountPrice ?? i.Course?.Price ?? 0,
                AddedAtUtc = i.AddedAtUtc
            }).ToList()
        };
    }
}
