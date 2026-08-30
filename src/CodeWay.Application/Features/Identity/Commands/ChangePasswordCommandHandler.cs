namespace CodeWay.Application.Features.Identity.Commands;

using CodeWay.Application.Contracts;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICurrentUserService _currentUser;

    public ChangePasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _currentUser = currentUser;
    }

    public async Task Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new DomainException("Not authenticated.");

        var user = await _unitOfWork.Users.GetByIdAsync(_currentUser.UserId.Value, cancellationToken)
            ?? throw new NotFoundException("User", _currentUser.UserId.Value);

        if (!_passwordHasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new DomainException("Current password is incorrect.");

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

        // Revoke all refresh tokens to force re-login everywhere after password change
        foreach (var rt in user.RefreshTokens.Where(t => t.RevokedAtUtc is null))
        {
            rt.RevokedAtUtc = DateTime.UtcNow;
            rt.RevokedByIp = "password-change";
        }

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
