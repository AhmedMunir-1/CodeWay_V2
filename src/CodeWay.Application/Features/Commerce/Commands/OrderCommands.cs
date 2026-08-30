namespace CodeWay.Application.Features.Commerce.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Commerce.DTOs;
using CodeWay.Domain.Entities.Commerce;
using CodeWay.Domain.Entities.Learning;
using CodeWay.Domain.Entities.Payments;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record CheckoutCommand(CheckoutDto Dto) : IRequest<OrderDto>;

public sealed record GetOrdersQuery(Guid? UserId = null) : IRequest<IReadOnlyList<OrderDto>>;

public sealed record GetOrderByIdQuery(Guid Id) : IRequest<OrderDto>;

public sealed class OrderCommandHandler :
    IRequestHandler<CheckoutCommand, OrderDto>,
    IRequestHandler<GetOrdersQuery, IReadOnlyList<OrderDto>>,
    IRequestHandler<GetOrderByIdQuery, OrderDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public OrderCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(CheckoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required to checkout.");

        var cartRepo = _unitOfWork.Repository<Cart>();
        var carts = await cartRepo.GetAsync(c => c.UserId == userId, cancellationToken);
        var cart = carts.FirstOrDefault()
            ?? throw new BusinessRuleViolationException("EmptyCart", "Cart is empty.");

        var cartItemRepo = _unitOfWork.Repository<CartItem>();
        var cartItems = (await cartItemRepo.GetAsync(i => i.CartId == cart.Id, cancellationToken)).ToList();

        if (cartItems.Count == 0)
            throw new BusinessRuleViolationException("EmptyCart", "Cart is empty.");

        var totalAmount = cartItems.Sum(i => i.UnitPrice);
        var discountAmount = 0m;

        // Apply Coupon if supplied
        if (!string.IsNullOrWhiteSpace(request.Dto.CouponCode))
        {
            var couponCode = request.Dto.CouponCode.Trim().ToUpperInvariant();
            var coupons = await _unitOfWork.Coupons.GetAsync(
                c => c.Code == couponCode && c.IsActive, cancellationToken);
            var coupon = coupons.FirstOrDefault();

            if (coupon != null &&
                coupon.ValidFromUtc <= DateTime.UtcNow &&
                coupon.ValidUntilUtc >= DateTime.UtcNow &&
                coupon.TimesUsed < coupon.MaxUses)
            {
                discountAmount = coupon.DiscountType == DiscountType.Percentage
                    ? totalAmount * (coupon.DiscountValue / 100m)
                    : Math.Min(totalAmount, coupon.DiscountValue);

                coupon.TimesUsed++;
                _unitOfWork.Coupons.Update(coupon);
            }
        }

        var finalAmount = Math.Max(0m, totalAmount - discountAmount);
        var orderNumber = $"ORD-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var order = new Order
        {
            UserId = userId,
            OrderNumber = orderNumber,
            TotalAmount = totalAmount,
            DiscountAmount = discountAmount,
            FinalAmount = finalAmount,
            Status = OrderStatus.Completed,
            CouponCode = request.Dto.CouponCode
        };

        foreach (var item in cartItems)
        {
            order.Items.Add(new OrderItem
            {
                OrderId = order.Id,
                CourseId = item.CourseId,
                UnitPrice = item.UnitPrice
            });

            // Automatically enroll user in purchased courses
            var alreadyEnrolled = await _unitOfWork.Enrollments.ExistsAsync(
                e => e.UserId == userId && e.CourseId == item.CourseId, cancellationToken);

            if (!alreadyEnrolled)
            {
                var enrollment = new Enrollment
                {
                    UserId = userId,
                    CourseId = item.CourseId,
                    Status = EnrollmentStatus.Active,
                    ProgressPercentage = 0,
                    EnrolledAtUtc = DateTime.UtcNow
                };
                await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);

                var course = await _unitOfWork.Courses.GetByIdAsync(item.CourseId, cancellationToken);
                if (course != null)
                {
                    course.TotalEnrollments++;
                    _unitOfWork.Courses.Update(course);
                }
            }
        }

        // Create Payment record
        var payment = new Payment
        {
            OrderId = order.Id,
            UserId = userId,
            Amount = finalAmount,
            Currency = "USD",
            Status = PaymentStatus.Completed,
            PaymentMethod = request.Dto.PaymentMethod,
            TransactionId = $"TXN-{Guid.NewGuid().ToString()[..12].ToUpper()}",
            PaidAtUtc = DateTime.UtcNow
        };
        await _unitOfWork.Payments.AddAsync(payment, cancellationToken);

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);

        // Clear Cart
        cartItemRepo.RemoveRange(cartItems);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<OrderDto>(order);
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _unitOfWork.Orders.GetAllAsync(cancellationToken);

        var query = orders.AsEnumerable();

        if (request.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == request.UserId.Value);
        }
        else if (_currentUser.UserId.HasValue)
        {
            query = query.Where(o => o.UserId == _currentUser.UserId.Value);
        }

        return _mapper.Map<IReadOnlyList<OrderDto>>(query.OrderByDescending(o => o.CreatedAtUtc).ToList());
    }

    public async Task<OrderDto> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Order", request.Id);

        return _mapper.Map<OrderDto>(order);
    }
}
