namespace CodeWay.Application.Features.Identity.Commands;

using MediatR;

/// <summary>Resets the user's password using a valid reset token.</summary>
public sealed record ResetPasswordCommand(
    string Token,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest;
