namespace CodeWay.Application.Features.Catalog.Queries;

using AutoMapper;
using CodeWay.Application.Common;
using CodeWay.Application.Features.Catalog.DTOs;
using CodeWay.Domain.Entities.Catalog;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record GetCategoriesQuery(bool? ActiveOnly = null, string? Search = null) : IRequest<IReadOnlyList<CategoryDto>>;

public sealed record GetCategoryByIdQuery(Guid Id) : IRequest<CategoryDto>;

public sealed class CategoryQueryHandler :
    IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>,
    IRequestHandler<GetCategoryByIdQuery, CategoryDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.Repository<Category>();
        var categories = await categoryRepo.GetAllAsync(cancellationToken);

        if (request.ActiveOnly.HasValue && request.ActiveOnly.Value)
        {
            categories = categories.Where(c => c.IsActive).ToList();
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            categories = categories.Where(c => c.Name.ToLower().Contains(search) || c.Slug.Contains(search)).ToList();
        }

        return _mapper.Map<IReadOnlyList<CategoryDto>>(categories);
    }

    public async Task<CategoryDto> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var categoryRepo = _unitOfWork.Repository<Category>();
        var category = await categoryRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Category", request.Id);

        return _mapper.Map<CategoryDto>(category);
    }
}
