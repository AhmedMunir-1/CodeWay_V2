namespace CodeWay.Application.Features.Learning.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Learning.DTOs;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Entities.Learning;
using CodeWay.Domain.Enums;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record EnrollCourseCommand(Guid CourseId) : IRequest<EnrollmentDto>;

public sealed record UpdateLessonProgressCommand(UpdateLessonProgressDto Dto) : IRequest<EnrollmentDto>;

public sealed record GetEnrollmentsQuery(Guid? UserId = null, Guid? CourseId = null) : IRequest<IReadOnlyList<EnrollmentDto>>;

public sealed record GetEnrollmentByIdQuery(Guid Id) : IRequest<EnrollmentDto>;

public sealed record GetCertificateQuery(Guid EnrollmentId) : IRequest<CertificateDto>;

public sealed class EnrollmentCommandHandler :
    IRequestHandler<EnrollCourseCommand, EnrollmentDto>,
    IRequestHandler<UpdateLessonProgressCommand, EnrollmentDto>,
    IRequestHandler<GetEnrollmentsQuery, IReadOnlyList<EnrollmentDto>>,
    IRequestHandler<GetEnrollmentByIdQuery, EnrollmentDto>,
    IRequestHandler<GetCertificateQuery, CertificateDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public EnrollmentCommandHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<EnrollmentDto> Handle(EnrollCourseCommand request, CancellationToken cancellationToken)
    {
        var userId = _currentUser.UserId
            ?? throw new DomainException("Authentication required to enroll in a course.");

        var course = await _unitOfWork.Courses.GetByIdAsync(request.CourseId, cancellationToken)
            ?? throw new NotFoundException("Course", request.CourseId);

        var alreadyEnrolled = await _unitOfWork.Enrollments.ExistsAsync(
            e => e.UserId == userId && e.CourseId == request.CourseId, cancellationToken);

        if (alreadyEnrolled)
            throw new ConflictException("Enrollment", "courseId", request.CourseId);

        var enrollment = new Enrollment
        {
            UserId = userId,
            CourseId = request.CourseId,
            Status = EnrollmentStatus.Active,
            ProgressPercentage = 0,
            EnrolledAtUtc = DateTime.UtcNow
        };

        await _unitOfWork.Enrollments.AddAsync(enrollment, cancellationToken);

        // Update Course Total Enrollments
        course.TotalEnrollments++;
        _unitOfWork.Courses.Update(course);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<EnrollmentDto> Handle(UpdateLessonProgressCommand request, CancellationToken cancellationToken)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.Dto.EnrollmentId, cancellationToken)
            ?? throw new NotFoundException("Enrollment", request.Dto.EnrollmentId);

        var lessonRepo = _unitOfWork.Repository<Lesson>();
        var lesson = await lessonRepo.GetByIdAsync(request.Dto.LessonId, cancellationToken)
            ?? throw new NotFoundException("Lesson", request.Dto.LessonId);

        var progressRepo = _unitOfWork.Repository<LessonProgress>();
        var existingProgressList = await progressRepo.GetAsync(
            p => p.EnrollmentId == request.Dto.EnrollmentId, cancellationToken);

        var existingProgress = existingProgressList.FirstOrDefault(p => p.LessonId == request.Dto.LessonId);

        if (existingProgress == null)
        {
            existingProgress = new LessonProgress
            {
                EnrollmentId = request.Dto.EnrollmentId,
                LessonId = request.Dto.LessonId,
                IsCompleted = request.Dto.IsCompleted,
                CompletedAtUtc = request.Dto.IsCompleted ? DateTime.UtcNow : null,
                LastWatchedPositionInSeconds = request.Dto.LastWatchedPositionInSeconds
            };
            await progressRepo.AddAsync(existingProgress, cancellationToken);
        }
        else
        {
            existingProgress.IsCompleted = request.Dto.IsCompleted;
            if (request.Dto.IsCompleted && existingProgress.CompletedAtUtc == null)
            {
                existingProgress.CompletedAtUtc = DateTime.UtcNow;
            }
            existingProgress.LastWatchedPositionInSeconds = request.Dto.LastWatchedPositionInSeconds;
            progressRepo.Update(existingProgress);
        }

        // Recalculate course progress
        var sectionRepo = _unitOfWork.Repository<Section>();
        var sections = await sectionRepo.GetAsync(s => s.CourseId == enrollment.CourseId, cancellationToken);
        var sectionIds = sections.Select(s => s.Id).ToList();

        var totalLessons = await lessonRepo.CountAsync(l => sectionIds.Contains(l.SectionId), cancellationToken);

        if (totalLessons > 0)
        {
            var completedLessonsCount = existingProgressList.Count(p => p.IsCompleted) + (existingProgress.IsCompleted && !existingProgressList.Any(p => p.LessonId == existingProgress.LessonId && p.IsCompleted) ? 1 : 0);
            enrollment.ProgressPercentage = Math.Min(100.0, Math.Round((double)completedLessonsCount / totalLessons * 100, 2));
        }

        if (enrollment.ProgressPercentage >= 100)
        {
            enrollment.Status = EnrollmentStatus.Completed;
            enrollment.CompletedAtUtc ??= DateTime.UtcNow;

            // Generate Certificate if not yet created
            var certRepo = _unitOfWork.Repository<Certificate>();
            var hasCert = await certRepo.ExistsAsync(c => c.EnrollmentId == enrollment.Id, cancellationToken);
            if (!hasCert)
            {
                var cert = new Certificate
                {
                    EnrollmentId = enrollment.Id,
                    CertificateCode = $"CW-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
                    IssuedAtUtc = DateTime.UtcNow,
                    PdfUrl = $"https://certificates.codeway.com/{enrollment.Id}.pdf"
                };
                await certRepo.AddAsync(cert, cancellationToken);
            }
        }

        _unitOfWork.Enrollments.Update(enrollment);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<IReadOnlyList<EnrollmentDto>> Handle(GetEnrollmentsQuery request, CancellationToken cancellationToken)
    {
        var enrollments = await _unitOfWork.Enrollments.GetAllAsync(cancellationToken);

        var query = enrollments.AsEnumerable();

        if (request.UserId.HasValue)
        {
            query = query.Where(e => e.UserId == request.UserId.Value);
        }
        else if (_currentUser.UserId.HasValue)
        {
            query = query.Where(e => e.UserId == _currentUser.UserId.Value);
        }

        if (request.CourseId.HasValue)
        {
            query = query.Where(e => e.CourseId == request.CourseId.Value);
        }

        return _mapper.Map<IReadOnlyList<EnrollmentDto>>(query.OrderByDescending(e => e.EnrolledAtUtc).ToList());
    }

    public async Task<EnrollmentDto> Handle(GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
    {
        var enrollment = await _unitOfWork.Enrollments.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Enrollment", request.Id);

        return _mapper.Map<EnrollmentDto>(enrollment);
    }

    public async Task<CertificateDto> Handle(GetCertificateQuery request, CancellationToken cancellationToken)
    {
        var certRepo = _unitOfWork.Repository<Certificate>();
        var certs = await certRepo.GetAsync(c => c.EnrollmentId == request.EnrollmentId, cancellationToken);
        var cert = certs.FirstOrDefault()
            ?? throw new NotFoundException("Certificate", request.EnrollmentId);

        return _mapper.Map<CertificateDto>(cert);
    }
}
