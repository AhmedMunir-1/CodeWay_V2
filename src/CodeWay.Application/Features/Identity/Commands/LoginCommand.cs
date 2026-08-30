namespace CodeWay.Application.Features.Identity.Commands;

using CodeWay.Application.Features.Identity.DTOs;
using MediatR;

/// <summary>Authenticates a user via email and password.</summary>
public sealed record LoginCommand(
    string Email,
    string Password,
    string? IpAddress = null
) : IRequest<AuthResponse>;
