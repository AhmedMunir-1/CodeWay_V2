namespace CodeWay.Application.Features.Identity.Commands;

using AutoMapper;
using CodeWay.Application.Contracts;
using CodeWay.Application.Features.Identity.DTOs;
using CodeWay.Domain.Entities.Identity;
using CodeWay.Domain.Exceptions;
using CodeWay.Domain.Interfaces;
using MediatR;

public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public RefreshTokenCommandHandler(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Extract userId from the (possibly expired) access token
        var userId = _tokenService.GetUserIdFromToken(request.AccessToken);
        if (userId is null)
            throw new DomainException("Invalid token.");

        var user = await _unitOfWork.Users.GetByIdAsync(userId.Value, cancellationToken)
            ?? throw new DomainException("Invalid token.");

        // Find the matching, non-revoked, non-expired refresh token
        var storedToken = user.RefreshTokens.FirstOrDefault(
            rt => rt.Token == request.RefreshToken &&
                  rt.RevokedAtUtc is null &&
                  rt.ExpiresAtUtc > DateTime.UtcNow);

        if (storedToken is null)
            throw new DomainException("Invalid or expired refresh token.");

        // Revoke old token
        var ip = request.IpAddress ?? "unknown";
        var newRefreshTokenValue = _tokenService.GenerateRefreshToken();

        storedToken.RevokedAtUtc = DateTime.UtcNow;
        storedToken.RevokedByIp = ip;
        storedToken.ReplacedByToken = newRefreshTokenValue;

        // Add new refresh token directly via generic repository (avoids navigation
        // collection mutation on a tracked entity causing DbUpdateConcurrencyException)
        var newRefreshToken = new RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddDays(30),
            CreatedByIp = ip
        };
        await _unitOfWork.Repository<RefreshToken>().AddAsync(newRefreshToken, cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return new AuthResponse
        {
            AccessToken = _tokenService.GenerateAccessToken(user),
            RefreshToken = newRefreshTokenValue,
            ExpiresAtUtc = DateTime.UtcNow.AddMinutes(60),
            User = _mapper.Map<UserProfileDto>(user)
        };
    }
}
