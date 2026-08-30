namespace CodeWay.Application.Features.Identity.Commands;

using CodeWay.Application.Contracts;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IDataProtectorTokenService _tokenService;
    private readonly ILogger<ResetPasswordCommandHandler> _logger;

    public ResetPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IDataProtectorTokenService tokenService,
        ILogger<ResetPasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var payload = _tokenService.ValidateToken("PasswordReset", request.Token);
        if (string.IsNullOrWhiteSpace(payload) || !Guid.TryParse(payload, out var userId))
        {
            throw new DomainException("Invalid or expired password reset token.");
        }

        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken)
            ?? throw new DomainException("Invalid or expired password reset token.");

        if (!user.IsActive)
        {
            throw new DomainException("This account is not active.");
        }

        user.PasswordHash = _passwordHasher.Hash(request.NewPassword);

        // Revoke all existing refresh tokens
        foreach (var rt in user.RefreshTokens.Where(t => t.RevokedAtUtc is null))
        {
            rt.RevokedAtUtc = DateTime.UtcNow;
            rt.RevokedByIp = "reset-password";
        }

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Password reset successfully for user: {UserId}", user.Id);
    }
}
