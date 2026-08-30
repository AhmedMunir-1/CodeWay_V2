namespace CodeWay.Application.Features.Catalog.Commands;

using AutoMapper;
using CodeWay.Application.Features.Catalog.DTOs;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record CreateLessonCommand(CreateLessonDto Dto) : IRequest<LessonDto>;

public sealed record UpdateLessonCommand(Guid Id, UpdateLessonDto Dto) : IRequest<LessonDto>;

public sealed record DeleteLessonCommand(Guid Id) : IRequest;

public sealed record GetLessonByIdQuery(Guid Id) : IRequest<LessonDto>;

public sealed record GetLessonsBySectionIdQuery(Guid SectionId) : IRequest<IReadOnlyList<LessonDto>>;

public sealed class LessonCommandHandler :
    IRequestHandler<CreateLessonCommand, LessonDto>,
    IRequestHandler<UpdateLessonCommand, LessonDto>,
    IRequestHandler<DeleteLessonCommand>,
    IRequestHandler<GetLessonByIdQuery, LessonDto>,
    IRequestHandler<GetLessonsBySectionIdQuery, IReadOnlyList<LessonDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LessonCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<LessonDto> Handle(CreateLessonCommand request, CancellationToken cancellationToken)
    {
        var sectionRepo = _unitOfWork.Repository<Section>();
        var sectionExists = await sectionRepo.ExistsAsync(s => s.Id == request.Dto.SectionId, cancellationToken);
        if (!sectionExists)
            throw new NotFoundException("Section", request.Dto.SectionId);

        var lessonRepo = _unitOfWork.Repository<Lesson>();
        var lesson = new Lesson
        {
            SectionId = request.Dto.SectionId,
            Title = request.Dto.Title.Trim(),
            Description = request.Dto.Description?.Trim(),
            DisplayOrder = request.Dto.DisplayOrder,
            DurationInSeconds = request.Dto.DurationInSeconds,
            Type = request.Dto.Type,
            ContentUrl = request.Dto.ContentUrl?.Trim(),
            ArticleBody = request.Dto.ArticleBody?.Trim(),
            IsFreePreview = request.Dto.IsFreePreview
        };

        await lessonRepo.AddAsync(lesson, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task<LessonDto> Handle(UpdateLessonCommand request, CancellationToken cancellationToken)
    {
        var lessonRepo = _unitOfWork.Repository<Lesson>();
        var lesson = await lessonRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Lesson", request.Id);

        lesson.Title = request.Dto.Title.Trim();
        lesson.Description = request.Dto.Description?.Trim();
        lesson.DisplayOrder = request.Dto.DisplayOrder;
        lesson.DurationInSeconds = request.Dto.DurationInSeconds;
        lesson.Type = request.Dto.Type;
        lesson.ContentUrl = request.Dto.ContentUrl?.Trim();
        lesson.ArticleBody = request.Dto.ArticleBody?.Trim();
        lesson.IsFreePreview = request.Dto.IsFreePreview;

        lessonRepo.Update(lesson);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task Handle(DeleteLessonCommand request, CancellationToken cancellationToken)
    {
        var lessonRepo = _unitOfWork.Repository<Lesson>();
        var lesson = await lessonRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Lesson", request.Id);

        lessonRepo.Remove(lesson);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<LessonDto> Handle(GetLessonByIdQuery request, CancellationToken cancellationToken)
    {
        var lessonRepo = _unitOfWork.Repository<Lesson>();
        var lesson = await lessonRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Lesson", request.Id);

        return _mapper.Map<LessonDto>(lesson);
    }

    public async Task<IReadOnlyList<LessonDto>> Handle(GetLessonsBySectionIdQuery request, CancellationToken cancellationToken)
    {
        var lessonRepo = _unitOfWork.Repository<Lesson>();
        var lessons = await lessonRepo.GetAsync(l => l.SectionId == request.SectionId, cancellationToken);
        return _mapper.Map<IReadOnlyList<LessonDto>>(lessons.OrderBy(l => l.DisplayOrder).ToList());
    }
}
