namespace CodeWay.Application.Features.Identity.Commands;

using AutoMapper;
using CodeWay.Application.Common;
using CodeWay.Application.Features.Identity.DTOs;
using CodeWay.Domain.Entities.Identity;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record GetUsersQuery(
    string? Search = null,
    bool? IsActive = null,
    int Page = 1,
    int PageSize = 10
) : IRequest<PaginatedResult<UserProfileDto>>;

public sealed record GetUserByIdQuery(Guid Id) : IRequest<UserProfileDto>;

public sealed record DeactivateUserCommand(Guid Id) : IRequest;

public sealed record ActivateUserCommand(Guid Id) : IRequest;

public sealed record DeleteUserCommand(Guid Id) : IRequest;

public sealed class UserManagementCommandHandler :
    IRequestHandler<GetUsersQuery, PaginatedResult<UserProfileDto>>,
    IRequestHandler<GetUserByIdQuery, UserProfileDto>,
    IRequestHandler<DeactivateUserCommand>,
    IRequestHandler<ActivateUserCommand>,
    IRequestHandler<DeleteUserCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UserManagementCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<PaginatedResult<UserProfileDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _unitOfWork.Users.GetAllAsync(cancellationToken);

        var query = users.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim().ToLower();
            query = query.Where(u => u.FirstName.ToLower().Contains(search) ||
                                     u.LastName.ToLower().Contains(search) ||
                                     u.Email.ToLower().Contains(search));
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        var totalCount = query.Count();
        var pagedItems = query
            .OrderByDescending(u => u.CreatedAtUtc)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var mapped = _mapper.Map<IReadOnlyList<UserProfileDto>>(pagedItems);
        return new PaginatedResult<UserProfileDto>(mapped, totalCount, request.Page, request.PageSize);
    }

    public async Task<UserProfileDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        return _mapper.Map<UserProfileDto>(user);
    }

    public async Task Handle(DeactivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        user.IsActive = false;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(ActivateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        user.IsActive = true;
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("User", request.Id);

        user.IsDeleted = true;
        user.DeletedAtUtc = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
