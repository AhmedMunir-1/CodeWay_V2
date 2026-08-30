namespace CodeWay.Application.Features.Identity.Queries;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Identity.DTOs;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, UserProfileDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;
    private readonly IMapper _mapper;

    public GetCurrentUserQueryHandler(
        IUnitOfWork unitOfWork,
        ICurrentUserService currentUser,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<UserProfileDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new DomainException("Not authenticated.");

        var user = await _unitOfWork.Users.GetByIdAsync(_currentUser.UserId.Value, cancellationToken)
            ?? throw new NotFoundException("User", _currentUser.UserId.Value);

        return _mapper.Map<UserProfileDto>(user);
    }
}
