namespace CodeWay.Application.Features.Learning.Validators;

using CodeWay.Application.Features.Learning.Commands;
using FluentValidation;

public class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator()
    {
        RuleFor(x => x.Dto.CourseId).NotEmpty().WithMessage("Course ID is required.");
        RuleFor(x => x.Dto.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Dto.Comment)
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Dto.Comment));
    }
}

public class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Review ID is required.");
        RuleFor(x => x.Dto.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
        RuleFor(x => x.Dto.Comment)
            .MaximumLength(1000).WithMessage("Comment cannot exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Dto.Comment));
    }
}

public class UpdateLessonProgressCommandValidator : AbstractValidator<UpdateLessonProgressCommand>
{
    public UpdateLessonProgressCommandValidator()
    {
        RuleFor(x => x.Dto.EnrollmentId).NotEmpty().WithMessage("Enrollment ID is required.");
        RuleFor(x => x.Dto.LessonId).NotEmpty().WithMessage("Lesson ID is required.");
        RuleFor(x => x.Dto.LastWatchedPositionInSeconds)
            .GreaterThanOrEqualTo(0).WithMessage("Last watched position cannot be negative.");
    }
}
