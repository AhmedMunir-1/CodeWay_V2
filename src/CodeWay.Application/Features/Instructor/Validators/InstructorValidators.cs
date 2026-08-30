namespace CodeWay.Application.Features.Instructor.Validators;

using CodeWay.Application.Features.Instructor.Commands;
using FluentValidation;

public class CreateInstructorProfileCommandValidator : AbstractValidator<CreateInstructorProfileCommand>
{
    public CreateInstructorProfileCommandValidator()
    {
        RuleFor(x => x.Dto.Headline)
            .NotEmpty().WithMessage("Headline is required.")
            .MaximumLength(150).WithMessage("Headline cannot exceed 150 characters.");

        RuleFor(x => x.Dto.Biography)
            .NotEmpty().WithMessage("Biography is required.")
            .MaximumLength(2000).WithMessage("Biography cannot exceed 2000 characters.");

        RuleFor(x => x.Dto.PayoutEmail)
            .NotEmpty().WithMessage("Payout email is required.")
            .EmailAddress().WithMessage("A valid payout email address is required.");
    }
}

public class UpdateInstructorProfileCommandValidator : AbstractValidator<UpdateInstructorProfileCommand>
{
    public UpdateInstructorProfileCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Profile ID is required.");

        RuleFor(x => x.Dto.Headline)
            .NotEmpty().WithMessage("Headline is required.")
            .MaximumLength(150).WithMessage("Headline cannot exceed 150 characters.");

        RuleFor(x => x.Dto.Biography)
            .NotEmpty().WithMessage("Biography is required.")
            .MaximumLength(2000).WithMessage("Biography cannot exceed 2000 characters.");

        RuleFor(x => x.Dto.PayoutEmail)
            .NotEmpty().WithMessage("Payout email is required.")
            .EmailAddress().WithMessage("A valid payout email address is required.");
    }
}

public class CreatePayoutRequestCommandValidator : AbstractValidator<CreatePayoutRequestCommand>
{
    public CreatePayoutRequestCommandValidator()
    {
        RuleFor(x => x.Dto.Amount)
            .GreaterThan(0).WithMessage("Payout amount must be greater than zero.");

        RuleFor(x => x.Dto.PayoutMethod)
            .NotEmpty().WithMessage("Payout method is required.");
    }
}
