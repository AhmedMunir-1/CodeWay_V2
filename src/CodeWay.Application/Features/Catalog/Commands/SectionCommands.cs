namespace CodeWay.Application.Features.Catalog.Commands;

using AutoMapper;
using CodeWay.Application.Features.Catalog.DTOs;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record CreateSectionCommand(CreateSectionDto Dto) : IRequest<SectionDto>;

public sealed record UpdateSectionCommand(Guid Id, UpdateSectionDto Dto) : IRequest<SectionDto>;

public sealed record DeleteSectionCommand(Guid Id) : IRequest;

public sealed record GetSectionsByCourseIdQuery(Guid CourseId) : IRequest<IReadOnlyList<SectionDto>>;

public sealed class SectionCommandHandler :
    IRequestHandler<CreateSectionCommand, SectionDto>,
    IRequestHandler<UpdateSectionCommand, SectionDto>,
    IRequestHandler<DeleteSectionCommand>,
    IRequestHandler<GetSectionsByCourseIdQuery, IReadOnlyList<SectionDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SectionCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<SectionDto> Handle(CreateSectionCommand request, CancellationToken cancellationToken)
    {
        var courseExists = await _unitOfWork.Courses.ExistsAsync(c => c.Id == request.Dto.CourseId, cancellationToken);
        if (!courseExists)
            throw new NotFoundException("Course", request.Dto.CourseId);

        var sectionRepo = _unitOfWork.Repository<Section>();
        var section = new Section
        {
            CourseId = request.Dto.CourseId,
            Title = request.Dto.Title.Trim(),
            Description = request.Dto.Description?.Trim(),
            DisplayOrder = request.Dto.DisplayOrder
        };

        await sectionRepo.AddAsync(section, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SectionDto>(section);
    }

    public async Task<SectionDto> Handle(UpdateSectionCommand request, CancellationToken cancellationToken)
    {
        var sectionRepo = _unitOfWork.Repository<Section>();
        var section = await sectionRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Section", request.Id);

        section.Title = request.Dto.Title.Trim();
        section.Description = request.Dto.Description?.Trim();
        section.DisplayOrder = request.Dto.DisplayOrder;

        sectionRepo.Update(section);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<SectionDto>(section);
    }

    public async Task Handle(DeleteSectionCommand request, CancellationToken cancellationToken)
    {
        var sectionRepo = _unitOfWork.Repository<Section>();
        var section = await sectionRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Section", request.Id);

        sectionRepo.Remove(section);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<SectionDto>> Handle(GetSectionsByCourseIdQuery request, CancellationToken cancellationToken)
    {
        var sectionRepo = _unitOfWork.Repository<Section>();
        var sections = await sectionRepo.GetAsync(s => s.CourseId == request.CourseId, cancellationToken);
        return _mapper.Map<IReadOnlyList<SectionDto>>(sections.OrderBy(s => s.DisplayOrder).ToList());
    }
}
