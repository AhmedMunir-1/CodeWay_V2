namespace CodeWay.Application.Features.Identity.DTOs;

/// <summary>Response returned after successful registration or login.</summary>
public sealed class AuthResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAtUtc { get; init; }
    public UserProfileDto User { get; init; } = null!;
}
