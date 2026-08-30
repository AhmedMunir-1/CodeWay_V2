namespace CodeWay.Application.Features.Catalog.Validators;

using CodeWay.Application.Features.Catalog.Commands;
using FluentValidation;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Dto.Slug)
            .NotEmpty().WithMessage("Category slug is required.")
            .MaximumLength(100).WithMessage("Category slug cannot exceed 100 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be lowercase alphanumeric with hyphens.");

        RuleFor(x => x.Dto.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Dto.Description));
    }
}

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommand>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Category ID is required.");

        RuleFor(x => x.Dto.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot exceed 100 characters.");

        RuleFor(x => x.Dto.Slug)
            .NotEmpty().WithMessage("Category slug is required.")
            .MaximumLength(100).WithMessage("Category slug cannot exceed 100 characters.")
            .Matches(@"^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug must be lowercase alphanumeric with hyphens.");
    }
}

public class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
{
    public CreateCourseCommandValidator()
    {
        RuleFor(x => x.Dto.Title)
            .NotEmpty().WithMessage("Course title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Dto.Description)
            .NotEmpty().WithMessage("Course description is required.");

        RuleFor(x => x.Dto.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        RuleFor(x => x.Dto.DiscountPrice)
            .GreaterThanOrEqualTo(0).WithMessage("Discount price cannot be negative.")
            .LessThanOrEqualTo(x => x.Dto.Price).WithMessage("Discount price cannot be higher than regular price.")
            .When(x => x.Dto.DiscountPrice.HasValue);

        RuleFor(x => x.Dto.CategoryId)
            .NotEmpty().WithMessage("Category is required.");

        RuleFor(x => x.Dto.InstructorId)
            .NotEmpty().WithMessage("Instructor is required.");
    }
}

public class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
{
    public UpdateCourseCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Course ID is required.");

        RuleFor(x => x.Dto.Title)
            .NotEmpty().WithMessage("Course title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.Dto.Description)
            .NotEmpty().WithMessage("Course description is required.");

        RuleFor(x => x.Dto.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

        RuleFor(x => x.Dto.CategoryId)
            .NotEmpty().WithMessage("Category is required.");
    }
}

public class CreateSectionCommandValidator : AbstractValidator<CreateSectionCommand>
{
    public CreateSectionCommandValidator()
    {
        RuleFor(x => x.Dto.CourseId).NotEmpty().WithMessage("Course ID is required.");
        RuleFor(x => x.Dto.Title)
            .NotEmpty().WithMessage("Section title is required.")
            .MaximumLength(150).WithMessage("Title cannot exceed 150 characters.");
    }
}

public class CreateLessonCommandValidator : AbstractValidator<CreateLessonCommand>
{
    public CreateLessonCommandValidator()
    {
        RuleFor(x => x.Dto.SectionId).NotEmpty().WithMessage("Section ID is required.");
        RuleFor(x => x.Dto.Title)
            .NotEmpty().WithMessage("Lesson title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");
        RuleFor(x => x.Dto.DurationInSeconds)
            .GreaterThanOrEqualTo(0).WithMessage("Duration cannot be negative.");
    }
}
