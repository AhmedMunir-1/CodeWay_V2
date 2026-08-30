namespace CodeWay.Application.Features.Commerce.Commands;

using AutoMapper;
using CodeWay.Application.Features.Commerce.DTOs;
using CodeWay.Domain.Entities.Commerce;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record CreateCouponCommand(CreateCouponDto Dto) : IRequest<CouponDto>;

public sealed record UpdateCouponCommand(Guid Id, UpdateCouponDto Dto) : IRequest<CouponDto>;

public sealed record DeleteCouponCommand(Guid Id) : IRequest;

public sealed record GetCouponsQuery : IRequest<IReadOnlyList<CouponDto>>;

public sealed record GetCouponByIdQuery(Guid Id) : IRequest<CouponDto>;

public sealed record ValidateCouponQuery(string Code, decimal CartTotal) : IRequest<CouponValidationResultDto>;

public sealed class CouponCommandHandler :
    IRequestHandler<CreateCouponCommand, CouponDto>,
    IRequestHandler<UpdateCouponCommand, CouponDto>,
    IRequestHandler<DeleteCouponCommand>,
    IRequestHandler<GetCouponsQuery, IReadOnlyList<CouponDto>>,
    IRequestHandler<GetCouponByIdQuery, CouponDto>,
    IRequestHandler<ValidateCouponQuery, CouponValidationResultDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CouponCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CouponDto> Handle(CreateCouponCommand request, CancellationToken cancellationToken)
    {
        var code = request.Dto.Code.Trim().ToUpperInvariant();
        var exists = await _unitOfWork.Coupons.ExistsAsync(c => c.Code == code, cancellationToken);
        if (exists)
            throw new ConflictException("Coupon", "code", code);

        var coupon = new Coupon
        {
            Code = code,
            DiscountType = request.Dto.DiscountType,
            DiscountValue = request.Dto.DiscountValue,
            MaxUses = request.Dto.MaxUses,
            TimesUsed = 0,
            ValidFromUtc = request.Dto.ValidFromUtc,
            ValidUntilUtc = request.Dto.ValidUntilUtc,
            IsActive = request.Dto.IsActive
        };

        await _unitOfWork.Coupons.AddAsync(coupon, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CouponDto>(coupon);
    }

    public async Task<CouponDto> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Coupon", request.Id);

        coupon.DiscountType = request.Dto.DiscountType;
        coupon.DiscountValue = request.Dto.DiscountValue;
        coupon.MaxUses = request.Dto.MaxUses;
        coupon.ValidFromUtc = request.Dto.ValidFromUtc;
        coupon.ValidUntilUtc = request.Dto.ValidUntilUtc;
        coupon.IsActive = request.Dto.IsActive;

        _unitOfWork.Coupons.Update(coupon);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CouponDto>(coupon);
    }

    public async Task Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Coupon", request.Id);

        _unitOfWork.Coupons.Remove(coupon);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<CouponDto>> Handle(GetCouponsQuery request, CancellationToken cancellationToken)
    {
        var coupons = await _unitOfWork.Coupons.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<CouponDto>>(coupons.OrderByDescending(c => c.CreatedAtUtc).ToList());
    }

    public async Task<CouponDto> Handle(GetCouponByIdQuery request, CancellationToken cancellationToken)
    {
        var coupon = await _unitOfWork.Coupons.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Coupon", request.Id);

        return _mapper.Map<CouponDto>(coupon);
    }

    public async Task<CouponValidationResultDto> Handle(ValidateCouponQuery request, CancellationToken cancellationToken)
    {
        var code = request.Code.Trim().ToUpperInvariant();
        var coupons = await _unitOfWork.Coupons.GetAsync(c => c.Code == code, cancellationToken);
        var coupon = coupons.FirstOrDefault();

        if (coupon == null || !coupon.IsActive)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Invalid coupon code." };
        }

        if (DateTime.UtcNow < coupon.ValidFromUtc)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon is not yet active." };
        }

        if (DateTime.UtcNow > coupon.ValidUntilUtc)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon has expired." };
        }

        if (coupon.TimesUsed >= coupon.MaxUses)
        {
            return new CouponValidationResultDto { IsValid = false, Message = "Coupon usage limit reached." };
        }

        var discount = coupon.DiscountType == DiscountType.Percentage
            ? request.CartTotal * (coupon.DiscountValue / 100m)
            : Math.Min(request.CartTotal, coupon.DiscountValue);

        return new CouponValidationResultDto
        {
            IsValid = true,
            Message = "Coupon applied successfully.",
            DiscountType = coupon.DiscountType,
            DiscountValue = coupon.DiscountValue,
            DiscountCalculated = Math.Round(discount, 2)
        };
    }
}
