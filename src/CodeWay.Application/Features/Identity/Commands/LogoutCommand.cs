namespace CodeWay.Application.Features.Identity.Commands;

using MediatR;

/// <summary>Revokes the user's refresh token, effectively logging them out.</summary>
public sealed record LogoutCommand(
    string RefreshToken,
    string? IpAddress = null
) : IRequest;
