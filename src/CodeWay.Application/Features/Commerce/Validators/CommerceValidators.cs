namespace CodeWay.Application.Features.Commerce.Validators;

using CodeWay.Application.Features.Commerce.Commands;
using FluentValidation;

public class CreateCouponCommandValidator : AbstractValidator<CreateCouponCommand>
{
    public CreateCouponCommandValidator()
    {
        RuleFor(x => x.Dto.Code)
            .NotEmpty().WithMessage("Coupon code is required.")
            .MaximumLength(50).WithMessage("Coupon code cannot exceed 50 characters.");

        RuleFor(x => x.Dto.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than zero.");

        RuleFor(x => x.Dto.MaxUses)
            .GreaterThan(0).WithMessage("Max uses must be greater than zero.");

        RuleFor(x => x.Dto.ValidUntilUtc)
            .GreaterThan(x => x.Dto.ValidFromUtc).WithMessage("Valid until date must be after valid from date.");
    }
}

public class UpdateCouponCommandValidator : AbstractValidator<UpdateCouponCommand>
{
    public UpdateCouponCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Coupon ID is required.");

        RuleFor(x => x.Dto.DiscountValue)
            .GreaterThan(0).WithMessage("Discount value must be greater than zero.");

        RuleFor(x => x.Dto.MaxUses)
            .GreaterThan(0).WithMessage("Max uses must be greater than zero.");

        RuleFor(x => x.Dto.ValidUntilUtc)
            .GreaterThan(x => x.Dto.ValidFromUtc).WithMessage("Valid until date must be after valid from date.");
    }
}

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.Dto.CourseId).NotEmpty().WithMessage("Course ID is required.");
    }
}

public class AddToWishlistCommandValidator : AbstractValidator<AddToWishlistCommand>
{
    public AddToWishlistCommandValidator()
    {
        RuleFor(x => x.Dto.CourseId).NotEmpty().WithMessage("Course ID is required.");
    }
}
