namespace CodeWay.Application.Features.Identity.Commands;

using CodeWay.Application.Features.Identity.DTOs;
using MediatR;

/// <summary>Registers a new user account.</summary>
public sealed record RegisterCommand(
    string FirstName,
    string LastName,
    string Email,
    string Password,
    string ConfirmPassword,
    string? PhoneNumber = null,
    string? Bio = null
) : IRequest<AuthResponse>;
