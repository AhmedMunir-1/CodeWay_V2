namespace CodeWay.Application.Features.Identity.Commands;

using AutoMapper;
using CodeWay.Application.Features.Identity.DTOs;
using CodeWay.Domain.Entities.Identity;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed record GetRolesQuery : IRequest<IReadOnlyList<RoleDto>>;

public sealed record GetRoleByIdQuery(Guid Id) : IRequest<RoleDto>;

public sealed record CreateRoleCommand(CreateRoleDto Dto) : IRequest<RoleDto>;

public sealed record UpdateRoleCommand(Guid Id, UpdateRoleDto Dto) : IRequest<RoleDto>;

public sealed record DeleteRoleCommand(Guid Id) : IRequest;

public sealed record AssignRoleToUserCommand(AssignRoleDto Dto) : IRequest;

public sealed record RemoveRoleFromUserCommand(Guid UserId, Guid RoleId) : IRequest;

public sealed record GetUserRolesQuery(Guid UserId) : IRequest<IReadOnlyList<RoleDto>>;

public sealed class RoleCommandHandler :
    IRequestHandler<GetRolesQuery, IReadOnlyList<RoleDto>>,
    IRequestHandler<GetRoleByIdQuery, RoleDto>,
    IRequestHandler<CreateRoleCommand, RoleDto>,
    IRequestHandler<UpdateRoleCommand, RoleDto>,
    IRequestHandler<DeleteRoleCommand>,
    IRequestHandler<AssignRoleToUserCommand>,
    IRequestHandler<RemoveRoleFromUserCommand>,
    IRequestHandler<GetUserRolesQuery, IReadOnlyList<RoleDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public RoleCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roleRepo = _unitOfWork.Repository<Role>();
        var roles = await roleRepo.GetAllAsync(cancellationToken);
        return _mapper.Map<IReadOnlyList<RoleDto>>(roles.OrderBy(r => r.Name).ToList());
    }

    public async Task<RoleDto> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
    {
        var roleRepo = _unitOfWork.Repository<Role>();
        var role = await roleRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Role", request.Id);

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleRepo = _unitOfWork.Repository<Role>();
        var normalizedName = request.Dto.Name.Trim().ToUpperInvariant();

        var exists = await roleRepo.ExistsAsync(r => r.NormalizedName == normalizedName, cancellationToken);
        if (exists)
            throw new ConflictException("Role", "name", request.Dto.Name);

        var role = new Role
        {
            Name = request.Dto.Name.Trim(),
            NormalizedName = normalizedName,
            Description = request.Dto.Description?.Trim()
        };

        await roleRepo.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var roleRepo = _unitOfWork.Repository<Role>();
        var role = await roleRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Role", request.Id);

        var normalizedName = request.Dto.Name.Trim().ToUpperInvariant();
        if (normalizedName != role.NormalizedName)
        {
            var exists = await roleRepo.ExistsAsync(r => r.NormalizedName == normalizedName && r.Id != request.Id, cancellationToken);
            if (exists)
                throw new ConflictException("Role", "name", request.Dto.Name);
        }

        role.Name = request.Dto.Name.Trim();
        role.NormalizedName = normalizedName;
        role.Description = request.Dto.Description?.Trim();

        roleRepo.Update(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return _mapper.Map<RoleDto>(role);
    }

    public async Task Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
    {
        var roleRepo = _unitOfWork.Repository<Role>();
        var role = await roleRepo.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException("Role", request.Id);

        var userRoleRepo = _unitOfWork.Repository<UserRole>();
        var hasUsers = await userRoleRepo.ExistsAsync(ur => ur.RoleId == request.Id, cancellationToken);
        if (hasUsers)
            throw new BusinessRuleViolationException("RoleInUse", "Cannot delete role currently assigned to users.");

        roleRepo.Remove(role);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var userExists = await _unitOfWork.Users.ExistsAsync(u => u.Id == request.Dto.UserId, cancellationToken);
        if (!userExists)
            throw new NotFoundException("User", request.Dto.UserId);

        var roleRepo = _unitOfWork.Repository<Role>();
        var roleExists = await roleRepo.ExistsAsync(r => r.Id == request.Dto.RoleId, cancellationToken);
        if (!roleExists)
            throw new NotFoundException("Role", request.Dto.RoleId);

        var userRoleRepo = _unitOfWork.Repository<UserRole>();
        var alreadyAssigned = await userRoleRepo.ExistsAsync(
            ur => ur.UserId == request.Dto.UserId && ur.RoleId == request.Dto.RoleId, cancellationToken);

        if (alreadyAssigned)
            return; // Idempotent

        var userRole = new UserRole
        {
            UserId = request.Dto.UserId,
            RoleId = request.Dto.RoleId
        };

        await userRoleRepo.AddAsync(userRole, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task Handle(RemoveRoleFromUserCommand request, CancellationToken cancellationToken)
    {
        var userRoleRepo = _unitOfWork.Repository<UserRole>();
        var userRoles = await userRoleRepo.GetAsync(
            ur => ur.UserId == request.UserId && ur.RoleId == request.RoleId, cancellationToken);

        var userRole = userRoles.FirstOrDefault();
        if (userRole != null)
        {
            userRoleRepo.Remove(userRole);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task<IReadOnlyList<RoleDto>> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var userRoleRepo = _unitOfWork.Repository<UserRole>();
        var userRoles = await userRoleRepo.GetAsync(ur => ur.UserId == request.UserId, cancellationToken);
        var roleIds = userRoles.Select(ur => ur.RoleId).ToList();

        var roleRepo = _unitOfWork.Repository<Role>();
        var roles = await roleRepo.GetAsync(r => roleIds.Contains(r.Id), cancellationToken);

        return _mapper.Map<IReadOnlyList<RoleDto>>(roles);
    }
}
