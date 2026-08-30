namespace CodeWay.Application.Features.Identity.Commands;

using CodeWay.Application.Contracts;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public LogoutCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new DomainException("Not authenticated.");

        var user = await _unitOfWork.Users.GetByIdAsync(_currentUser.UserId.Value, cancellationToken);
        if (user is null) return;

        var token = user.RefreshTokens.FirstOrDefault(
            rt => rt.Token == request.RefreshToken && rt.RevokedAtUtc is null);

        if (token is not null)
        {
            token.RevokedAtUtc = DateTime.UtcNow;
            token.RevokedByIp = request.IpAddress ?? "unknown";
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
