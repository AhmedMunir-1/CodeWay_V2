namespace CodeWay.Application.Features.Commerce.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Commerce.DTOs;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Entities.Commerce;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record GetCartQuery : IRequest<CartDto>;

public sealed record AddToCartCommand(AddToCartDto Dto) : IRequest<CartDto>;

public sealed record RemoveFromCartCommand(Guid CartItemId) : IRequest<CartDto>;

public sealed record ClearCartCommand : IRequest;

public sealed class CartCommandHandler :
    IRequestHandler<GetCartQuery, CartDto>,
    IRequestHandler<AddToCartCommand, CartDto>,
    IRequestHandler<RemoveFromCartCommand, CartDto>,
    IRequestHandler<ClearCartCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public CartCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required to access cart.");

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);
        return MapCartDto(cart);
    }

    public async Task<CartDto> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required to add items to cart.");

        var course = await _unitOfWork.Courses.GetByIdAsync(request.Dto.CourseId, cancellationToken)
            ?? throw new NotFoundException("Course", request.Dto.CourseId);

        // Check if user is already enrolled
        var alreadyEnrolled = await _unitOfWork.Enrollments.ExistsAsync(
            e => e.UserId == userId && e.CourseId == request.Dto.CourseId, cancellationToken);
        if (alreadyEnrolled)
            throw new BusinessRuleViolationException("AlreadyEnrolled", "You are already enrolled in this course.");

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);

        if (cart.Items.Any(i => i.CourseId == request.Dto.CourseId))
            throw new ConflictException("CartItem", "courseId", request.Dto.CourseId);

        var price = course.DiscountPrice ?? course.Price;
        var cartItem = new CartItem
        {
            CartId = cart.Id,
            CourseId = course.Id,
            UnitPrice = price,
            Course = course
        };

        cart.Items.Add(cartItem);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapCartDto(cart);
    }

    public async Task<CartDto> Handle(RemoveFromCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required.");

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);

        var item = cart.Items.FirstOrDefault(i => i.Id == request.CartItemId)
            ?? throw new NotFoundException("CartItem", request.CartItemId);

        var cartItemRepo = _unitOfWork.Repository<CartItem>();
        cartItemRepo.Remove(item);
        cart.Items.Remove(item);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapCartDto(cart);
    }

    public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required.");

        var cart = await GetOrCreateCartAsync(userId, cancellationToken);

        var cartItemRepo = _unitOfWork.Repository<CartItem>();
        cartItemRepo.RemoveRange(cart.Items);
        cart.Items.Clear();

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task<Cart> GetOrCreateCartAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cartRepo = _unitOfWork.Repository<Cart>();
        var carts = await cartRepo.GetAsync(c => c.UserId == userId, cancellationToken);
        var cart = carts.FirstOrDefault();

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };
            await cartRepo.AddAsync(cart, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        // Load items with Course navigation
        var cartItemRepo = _unitOfWork.Repository<CartItem>();
        var items = await cartItemRepo.GetAsync(i => i.CartId == cart.Id, cancellationToken);
        var courseRepo = _unitOfWork.Courses;

        foreach (var item in items)
        {
            if (item.Course == null)
            {
                item.Course = (await courseRepo.GetByIdAsync(item.CourseId, cancellationToken))!;
            }
        }

        cart.Items = items.ToList();
        return cart;
    }

    private static CartDto MapCartDto(Cart cart)
    {
        var items = cart.Items.Select(i => new CartItemDto
        {
            Id = i.Id,
            CourseId = i.CourseId,
            CourseTitle = i.Course?.Title ?? string.Empty,
            CourseSlug = i.Course?.Slug ?? string.Empty,
            CourseThumbnailUrl = i.Course?.ThumbnailUrl,
            UnitPrice = i.UnitPrice
        }).ToList();

        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            TotalAmount = items.Sum(i => i.UnitPrice),
            Items = items
        };
    }
}
