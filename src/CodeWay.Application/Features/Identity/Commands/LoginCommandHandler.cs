namespace CodeWay.Application.Features.Identity.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Identity.DTOs;
using CodeWay.Domain.Entities.Identity;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IMapper mapper,
        ILogger<LoginCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(
            request.Email.Trim().ToLowerInvariant(), cancellationToken);

        // Deliberately generic message — do not reveal whether email exists
        const string genericError = "Invalid credentials.";

        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            throw new DomainException(genericError);

        if (!user.IsActive)
            throw new DomainException("This account has been deactivated. Please contact support.");

        // Add a fresh refresh token directly via generic repository (bypasses
        // the unloaded navigation collection to avoid EF concurrency issues).
        var ip = request.IpAddress ?? "unknown";
        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30),
            CreatedByIp = ip
        };

        await _unitOfWork.Repository<RefreshToken>().AddAsync(refreshToken, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);

        _logger.LogInformation("User logged in: {Email}", user.Email);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(60),
            User = _mapper.Map<UserProfileDto>(user)
        };
    }
}
