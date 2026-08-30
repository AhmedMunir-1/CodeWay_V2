namespace CodeWay.Application.Features.Identity.Commands;

using MediatR;

/// <summary>
/// Initiates the forgot-password flow. Always returns a generic response
/// to prevent email enumeration — the caller never knows if the email exists.
/// </summary>
public sealed record ForgotPasswordCommand(string Email) : IRequest<string>;
