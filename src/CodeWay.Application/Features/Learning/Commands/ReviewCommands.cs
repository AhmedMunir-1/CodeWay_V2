namespace CodeWay.Application.Features.Learning.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Learning.DTOs;
using CodeWay.Domain.Entities.Learning;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record CreateReviewCommand(CreateReviewDto Dto) : IRequest<ReviewDto>;

public sealed record UpdateReviewCommand(Guid Id, UpdateReviewDto Dto) : IRequest<ReviewDto>;

public sealed record DeleteReviewCommand(Guid Id) : IRequest;

public sealed record GetReviewsByCourseIdQuery(Guid CourseId) : IRequest<IReadOnlyList<ReviewDto>>;

public sealed class ReviewCommandHandler :
    IRequestHandler<CreateReviewCommand, ReviewDto>,
    IRequestHandler<UpdateReviewCommand, ReviewDto>,
    IRequestHandler<DeleteReviewCommand>,
    IRequestHandler<GetReviewsByCourseIdQuery, IReadOnlyList<ReviewDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public ReviewCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<ReviewDto> Handle(CreateReviewCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required to submit a review.");

        var course = await _unitOfWork.Courses.GetByIdAsync(request.Dto.CourseId, cancellationToken)
            ?? throw new NotFoundException("Course", request.Dto.CourseId);

        var existingReviews = await _unitOfWork.Reviews.GetAsync(
            r => r.CourseId == request.Dto.CourseId && r.UserId == userId, cancellationToken);

        if (existingReviews.Any())
            throw new ConflictException("Review", "courseId", request.Dto.CourseId);

        var review = new Review
        {
            CourseId = request.Dto.CourseId,
            UserId = userId,
            Rating = request.Dto.Rating,
            Comment = request.Dto.Comment?.Trim()
        };

        await _unitOfWork.Reviews.AddAsync(review, cancellationToken);

        // Recalculate Course Average Rating & Total Reviews
        var allCourseReviews = await _unitOfWork.Reviews.GetAsync(r => r.CourseId == request.Dto.CourseId, cancellationToken);
        var ratings = allCourseReviews.Select(r => r.Rating).Concat([request.Dto.Rating]).ToList();
        course.TotalReviews = ratings.Count;
        course.AverageRating = Math.Round(ratings.Average(), 1);
        _unitOfWork.Courses.Update(course);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task<ReviewDto> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Review", request.Id);

        review.Rating = request.Dto.Rating;
        review.Comment = request.Dto.Comment?.Trim();

        _unitOfWork.Reviews.Update(review);

        // Recalculate Course Average Rating
        var course = await _unitOfWork.Courses.GetByIdAsync(review.CourseId, cancellationToken);
        if (course != null)
        {
            var allReviews = await _unitOfWork.Reviews.GetAsync(r => r.CourseId == review.CourseId, cancellationToken);
            if (allReviews.Any())
            {
                course.AverageRating = Math.Round(allReviews.Average(r => r.Id == review.Id ? request.Dto.Rating : r.Rating), 1);
                _unitOfWork.Courses.Update(course);
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ReviewDto>(review);
    }

    public async Task Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _unitOfWork.Reviews.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Review", request.Id);

        _unitOfWork.Reviews.Remove(review);

        var course = await _unitOfWork.Courses.GetByIdAsync(review.CourseId, cancellationToken);
        if (course != null)
        {
            var remainingReviews = (await _unitOfWork.Reviews.GetAsync(r => r.CourseId == review.CourseId && r.Id != review.Id, cancellationToken)).ToList();
            course.TotalReviews = remainingReviews.Count;
            course.AverageRating = remainingReviews.Count != 0 ? Math.Round(remainingReviews.Average(r => r.Rating), 1) : 0;
            _unitOfWork.Courses.Update(course);
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ReviewDto>> Handle(GetReviewsByCourseIdQuery request, CancellationToken cancellationToken)
    {
        var reviews = await _unitOfWork.Reviews.GetAsync(r => r.CourseId == request.CourseId, cancellationToken);
        return _mapper.Map<IReadOnlyList<ReviewDto>>(reviews.OrderByDescending(r => r.CreatedAtUtc).ToList());
    }
}
