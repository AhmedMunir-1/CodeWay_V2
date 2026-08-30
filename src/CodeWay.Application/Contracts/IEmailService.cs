namespace CodeWay.Application.Contracts;

/// <summary>
/// Contract for sending transactional emails.
/// Implemented in Infrastructure by <c>EmailService</c> using MailKit.
/// A dev stub implementation logs to console when SMTP is not configured.
/// </summary>
public interface IEmailService
{
    /// <summary>Sends a password reset email containing the reset link.</summary>
    Task SendPasswordResetEmailAsync(
        string toEmail,
        string resetLink,
        CancellationToken cancellationToken = default);

    /// <summary>Sends an email confirmation link to the user.</summary>
    Task SendEmailConfirmationAsync(
        string toEmail,
        string confirmationLink,
        CancellationToken cancellationToken = default);

    /// <summary>Sends a generic email with a subject and body.</summary>
    Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default);
}
