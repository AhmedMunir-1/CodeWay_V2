namespace CodeWay.Application.Features.Identity.Commands;

using MediatR;

/// <summary>Updates the current user's profile fields.</summary>
public sealed record UpdateProfileCommand(
    string FirstName,
    string LastName,
    string? PhoneNumber,
    string? Bio,
    string? ProfilePictureUrl
) : IRequest;
