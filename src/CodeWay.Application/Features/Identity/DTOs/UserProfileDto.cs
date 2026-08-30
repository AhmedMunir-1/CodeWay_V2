namespace CodeWay.Application.Features.Identity.DTOs;

/// <summary>Read DTO — safe representation of a user's profile.</summary>
public sealed class UserProfileDto
{
    public Guid Id { get; init; }
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? PhoneNumber { get; init; }
    public string? ProfilePictureUrl { get; init; }
    public string? Bio { get; init; }
    public bool IsEmailConfirmed { get; init; }
    public bool IsActive { get; init; }
    public DateTime CreatedAtUtc { get; init; }
}
