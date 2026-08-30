namespace CodeWay.Application.Features.Identity.Commands;

using CodeWay.Application.Features.Identity.DTOs;
using MediatR;

/// <summary>Exchanges a valid refresh token for a new access + refresh token pair.</summary>
public sealed record RefreshTokenCommand(
    string AccessToken,
    string RefreshToken,
    string? IpAddress = null
) : IRequest<AuthResponse>;
