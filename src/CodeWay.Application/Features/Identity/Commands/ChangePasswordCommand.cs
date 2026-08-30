namespace CodeWay.Application.Features.Identity.Commands;

using MediatR;

/// <summary>Changes the current user's password after verifying the old password.</summary>
public sealed record ChangePasswordCommand(
    string CurrentPassword,
    string NewPassword,
    string ConfirmNewPassword
) : IRequest;
