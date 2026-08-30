namespace CodeWay.Application.Features.Catalog.Commands;

using AutoMapper;
using CodeWay.Application.Features.Catalog.DTOs;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Entities.Instructor;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using CodeWay.Domain.ValueObjects;
using MediatR;

public sealed record CreateCourseCommand(CreateCourseDto Dto) : IRequest<CourseDto>;

public sealed record UpdateCourseCommand(Guid Id, UpdateCourseDto Dto) : IRequest<CourseDto>;

public sealed record DeleteCourseCommand(Guid Id) : IRequest;

public sealed record PublishCourseCommand(Guid Id) : IRequest<CourseDto>;

public sealed class CourseCommandHandler :
    IRequestHandler<CreateCourseCommand, CourseDto>,
    IRequestHandler<UpdateCourseCommand, CourseDto>,
    IRequestHandler<DeleteCourseCommand>,
    IRequestHandler<PublishCourseCommand, CourseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CourseCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CourseDto> Handle(CreateCourseCommand request, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.Repository<Category>();
        var categoryExists = await categoryRepo.ExistsAsync(c => c.Id == request.Dto.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new NotFoundException("Category", request.Dto.CategoryId);

        var instructorExists = await _unitOfWork.InstructorProfiles.ExistsAsync(i => i.Id == request.Dto.InstructorId, cancellationToken);
        if (!instructorExists)
            throw new NotFoundException("InstructorProfile", request.Dto.InstructorId);

        var baseSlug = Slug.FromTitle(request.Dto.Title).Value;
        var slug = baseSlug;
        var suffix = 1;
        while (await _unitOfWork.Courses.IsSlugTakenAsync(slug, cancellationToken))
        {
            slug = $"{baseSlug}-{suffix++}";
        }

        var course = new Course
        {
            Title = request.Dto.Title.Trim(),
            Slug = slug,
            SubTitle = request.Dto.SubTitle?.Trim(),
            Description = request.Dto.Description.Trim(),
            Language = request.Dto.Language?.Trim() ?? "English",
            Level = request.Dto.Level,
            Status = CourseStatus.Draft,
            Price = request.Dto.Price,
            DiscountPrice = request.Dto.DiscountPrice,
            ThumbnailUrl = request.Dto.ThumbnailUrl?.Trim(),
            TrailerVideoUrl = request.Dto.TrailerVideoUrl?.Trim(),
            InstructorId = request.Dto.InstructorId,
            CategoryId = request.Dto.CategoryId
        };

        if (request.Dto.Requirements != null)
        {
            var order = 1;
            foreach (var req in request.Dto.Requirements.Where(r => !string.IsNullOrWhiteSpace(r)))
            {
                course.Requirements.Add(new CourseRequirement
                {
                    Requirement = req.Trim(),
                    DisplayOrder = order++
                });
            }
        }

        if (request.Dto.LearningOutcomes != null)
        {
            var order = 1;
            foreach (var outcome in request.Dto.LearningOutcomes.Where(o => !string.IsNullOrWhiteSpace(o)))
            {
                course.LearningOutcomes.Add(new CourseLearningOutcome
                {
                    Outcome = outcome.Trim(),
                    DisplayOrder = order++
                });
            }
        }

        await _unitOfWork.Courses.AddAsync(course, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CourseDto>(course);
    }

    public async Task<CourseDto> Handle(UpdateCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Course", request.Id);

        var categoryRepo = _unitOfWork.Repository<Category>();
        var categoryExists = await categoryRepo.ExistsAsync(c => c.Id == request.Dto.CategoryId, cancellationToken);
        if (!categoryExists)
            throw new NotFoundException("Category", request.Dto.CategoryId);

        course.Title = request.Dto.Title.Trim();
        course.SubTitle = request.Dto.SubTitle?.Trim();
        course.Description = request.Dto.Description.Trim();
        course.Language = request.Dto.Language?.Trim() ?? "English";
        course.Level = request.Dto.Level;
        course.Status = request.Dto.Status;
        course.Price = request.Dto.Price;
        course.DiscountPrice = request.Dto.DiscountPrice;
        course.ThumbnailUrl = request.Dto.ThumbnailUrl?.Trim();
        course.TrailerVideoUrl = request.Dto.TrailerVideoUrl?.Trim();
        course.CategoryId = request.Dto.CategoryId;

        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CourseDto>(course);
    }

    public async Task Handle(DeleteCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Course", request.Id);

        // Soft delete
        course.IsDeleted = true;
        course.DeletedAtUtc = DateTime.UtcNow;

        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<CourseDto> Handle(PublishCourseCommand request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Course", request.Id);

        // Invariant: cannot publish a course without title, description or price < 0
        if (string.IsNullOrWhiteSpace(course.Title) || string.IsNullOrWhiteSpace(course.Description))
            throw new BusinessRuleViolationException("CourseIncomplete", "Course must have title and description to be published.");

        course.Status = CourseStatus.Published;
        _unitOfWork.Courses.Update(course);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CourseDto>(course);
    }
}
