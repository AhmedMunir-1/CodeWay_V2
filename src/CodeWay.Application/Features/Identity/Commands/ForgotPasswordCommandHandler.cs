namespace CodeWay.Application.Features.Identity.Commands;

using CodeWay.Application.Contracts;
using CodeWay.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, string>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDataProtectorTokenService _tokenService;
    private readonly IEmailService _emailService;
    private readonly ILogger<ForgotPasswordCommandHandler> _logger;

    public ForgotPasswordCommandHandler(
        IUnitOfWork unitOfWork,
        IDataProtectorTokenService tokenService,
        IEmailService emailService,
        ILogger<ForgotPasswordCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task<string> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        const string genericResponse = "If the account exists, a password reset link has been sent.";

        var user = await _unitOfWork.Users.GetByEmailAsync(
            request.Email.Trim().ToLowerInvariant(), cancellationToken);

        if (user is null || !user.IsActive)
        {
            // Do not reveal user existence
            return genericResponse;
        }

        var token = _tokenService.GenerateToken("PasswordReset", user.Id.ToString());

        await _emailService.SendPasswordResetEmailAsync(user.Email, token, cancellationToken);

        _logger.LogInformation("Password reset token generated for user: {Email}", user.Email);

        return genericResponse;
    }
}
