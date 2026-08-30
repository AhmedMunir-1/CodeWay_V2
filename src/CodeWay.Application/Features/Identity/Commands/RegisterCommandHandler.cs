namespace CodeWay.Application.Features.Identity.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Identity.DTOs;
using CodeWay.Domain.Entities.Identity;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterCommandHandler> _logger;

    public RegisterCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        IMapper mapper,
        ILogger<RegisterCommandHandler> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        // Guard: duplicate email
        if (await _unitOfWork.Users.IsEmailTakenAsync(request.Email, cancellationToken))
            throw new ConflictException("User", "email", request.Email);

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = request.Email.Trim().ToLowerInvariant(),
            PasswordHash = _passwordHasher.Hash(request.Password),
            PhoneNumber = request.PhoneNumber?.Trim(),
            Bio = request.Bio?.Trim(),
            IsActive = true,
            IsEmailConfirmed = false
        };

        // Build refresh token and attach to navigation collection.
        // EF will INSERT both User and RefreshToken atomically in one SaveChangesAsync.
        var refreshTokenValue = _tokenService.GenerateRefreshToken();
        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30),
            CreatedByIp = "system"
        };
        user.RefreshTokens.Add(refreshToken);

        await _unitOfWork.Users.AddAsync(user, cancellationToken);

        // Single save — inserts User + RefreshToken atomically
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var accessToken = _tokenService.GenerateAccessToken(user);

        _logger.LogInformation("User registered: {Email}", user.Email);

        return new AuthResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(60),
            User = _mapper.Map<UserProfileDto>(user)
        };
    }
}
