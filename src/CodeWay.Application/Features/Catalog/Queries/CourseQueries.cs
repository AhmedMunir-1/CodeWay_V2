namespace CodeWay.Application.Features.Catalog.Queries;

using AutoMapper;
using CodeWay.Application.Common;
using CodeWay.Application.Features.Catalog.DTOs;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record GetCoursesQuery(
    string? Search = null,
    Guid? CategoryId = null,
    Guid? InstructorId = null,
    CourseLevel? Level = null,
    CourseStatus? Status = null,
    int Page = 1,
    int PageSize = 10,
    string? SortBy = null,
    bool SortDescending = false
) : IRequest<PaginatedResult<CourseDto>>;

public sealed record GetCourseByIdQuery(Guid Id) : IRequest<CourseDetailDto>;

public sealed record GetCourseBySlugQuery(string Slug) : IRequest<CourseDetailDto>;

public sealed class CourseQueryHandler :
    IRequestHandler<GetCoursesQuery, PaginatedResult<CourseDto>>,
    IRequestHandler<GetCourseByIdQuery, CourseDetailDto>,
    IRequestHandler<GetCourseBySlugQuery, CourseDetailDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CourseQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<CourseDto>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
    {
        var courses = await _unitOfWork.Courses.GetAllAsync(cancellationToken);

        var query = courses.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(c => c.Title.ToLower().Contains(search) || c.Description.ToLower().Contains(search));
        }

        if (request.CategoryId.HasValue)
        {
            query = query.Where(c => c.CategoryId == request.CategoryId.Value);
        }

        if (request.InstructorId.HasValue)
        {
            query = query.Where(c => c.InstructorId == request.InstructorId.Value);
        }

        if (request.Level.HasValue)
        {
            query = query.Where(c => c.Level == request.Level.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(c => c.Status == request.Status.Value);
        }

        query = request.SortBy?.ToLower() switch
        {
            "price" => request.SortDescending ? query.OrderByDescending(c => c.Price) : query.OrderBy(c => c.Price),
            "rating" => request.SortDescending ? query.OrderByDescending(c => c.AverageRating) : query.OrderBy(c => c.AverageRating),
            "title" => request.SortDescending ? query.OrderByDescending(c => c.Title) : query.OrderBy(c => c.Title),
            _ => request.SortDescending ? query.OrderByDescending(c => c.CreatedAtUtc) : query.OrderBy(c => c.CreatedAtUtc)
        };

        var totalCount = query.Count();
        var pagedItems = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var mapped = _mapper.Map<IReadOnlyList<CourseDto>>(pagedItems);
        return new PaginatedResult<CourseDto>(mapped, totalCount, request.Page, request.PageSize);
    }

    public async Task<CourseDetailDto> Handle(GetCourseByIdQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Course", request.Id);

        return _mapper.Map<CourseDetailDto>(course);
    }

    public async Task<CourseDetailDto> Handle(GetCourseBySlugQuery request, CancellationToken cancellationToken)
    {
        var course = await _unitOfWork.Courses.GetBySlugAsync(request.Slug, cancellationToken)
            ?? throw new NotFoundException("Course", request.Slug);

        return _mapper.Map<CourseDetailDto>(course);
    }
}
