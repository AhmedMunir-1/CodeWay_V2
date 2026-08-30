namespace CodeWay.Application.Features.Catalog.Commands;

using AutoMapper;
using CodeWay.Application.Features.Catalog.DTOs;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record CreateCategoryCommand(CreateCategoryDto Dto) : IRequest<CategoryDto>;

public sealed record UpdateCategoryCommand(Guid Id, UpdateCategoryDto Dto) : IRequest<CategoryDto>;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest;

public sealed class CategoryCommandHandler :
    IRequestHandler<CreateCategoryCommand, CategoryDto>,
    IRequestHandler<UpdateCategoryCommand, CategoryDto>,
    IRequestHandler<DeleteCategoryCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.Repository<Category>();
        var slug = request.Dto.Slug.Trim().ToLowerInvariant();

        if (await categoryRepo.ExistsAsync(c => c.Slug == slug, cancellationToken))
            throw new ConflictException("Category", "slug", slug);

        if (request.Dto.ParentCategoryId.HasValue)
        {
            var parentExists = await categoryRepo.ExistsAsync(c => c.Id == request.Dto.ParentCategoryId.Value, cancellationToken);
            if (!parentExists)
                throw new NotFoundException("Parent Category", request.Dto.ParentCategoryId.Value);
        }

        var category = new Category
        {
            Name = request.Dto.Name.Trim(),
            Slug = slug,
            Description = request.Dto.Description?.Trim(),
            ParentCategoryId = request.Dto.ParentCategoryId,
            IsActive = request.Dto.IsActive
        };

        await categoryRepo.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.Repository<Category>();
        var category = await categoryRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Category", request.Id);

        var slug = request.Dto.Slug.Trim().ToLowerInvariant();
        if (slug != category.Slug && await categoryRepo.ExistsAsync(c => c.Slug == slug && c.Id != request.Id, cancellationToken))
            throw new ConflictException("Category", "slug", slug);

        if (request.Dto.ParentCategoryId.HasValue && request.Dto.ParentCategoryId.Value != request.Id)
        {
            var parentExists = await categoryRepo.ExistsAsync(c => c.Id == request.Dto.ParentCategoryId.Value, cancellationToken);
            if (!parentExists)
                throw new NotFoundException("Parent Category", request.Dto.ParentCategoryId.Value);
        }

        category.Name = request.Dto.Name.Trim();
        category.Slug = slug;
        category.Description = request.Dto.Description?.Trim();
        category.ParentCategoryId = request.Dto.ParentCategoryId == request.Id ? null : request.Dto.ParentCategoryId;
        category.IsActive = request.Dto.IsActive;

        categoryRepo.Update(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<CategoryDto>(category);
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.Repository<Category>();
        var category = await categoryRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Category", request.Id);

        // Check if category has any courses
        var courseRepo = _unitOfWork.Courses;
        var hasCourses = await courseRepo.ExistsAsync(c => c.CategoryId == request.Id, cancellationToken);
        if (hasCourses)
            throw new BusinessRuleViolationException("CategoryHasCourses", "Cannot delete category with associated courses.");

        categoryRepo.Remove(category);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
