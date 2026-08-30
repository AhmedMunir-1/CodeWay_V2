namespace CodeWay.Infrastructure.Services;

using CodeWay.Application.Contracts;
using CodeWay.Infrastructure.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

public class EmailService : IEmailService
{
    private readonly EmailOptions _emailOptions;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> emailOptions, ILogger<EmailService> logger)
    {
        _emailOptions = emailOptions.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(
        string toEmail,
        string resetToken,
        CancellationToken cancellationToken = default)
    {
        var subject = "Reset Your CodeWay Password";
        var body = $@"
            <h2>Password Reset Request</h2>
            <p>You requested to reset your password. Use the following token to complete the process:</p>
            <p style='background:#f4f4f4;padding:10px;font-family:monospace;'>{resetToken}</p>
            <p>If you did not request this, please ignore this email.</p>";

        await SendAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendEmailConfirmationAsync(
        string toEmail,
        string confirmationToken,
        CancellationToken cancellationToken = default)
    {
        var subject = "Confirm Your CodeWay Email";
        var body = $@"
            <h2>Email Confirmation</h2>
            <p>Welcome to CodeWay! Use the following token to confirm your email:</p>
            <p style='background:#f4f4f4;padding:10px;font-family:monospace;'>{confirmationToken}</p>";

        await SendAsync(toEmail, subject, body, cancellationToken);
    }

    public async Task SendAsync(
        string toEmail,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("CodeWay", _emailOptions.From));
            message.To.Add(MailboxAddress.Parse(toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                _emailOptions.SmtpHost,
                _emailOptions.SmtpPort,
                _emailOptions.UseSsl,
                cancellationToken);

            if (!string.IsNullOrEmpty(_emailOptions.UserName) && !string.IsNullOrEmpty(_emailOptions.Password))
            {
                await client.AuthenticateAsync(_emailOptions.UserName, _emailOptions.Password, cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            await client.DisconnectAsync(true, cancellationToken);

            _logger.LogInformation("Email sent successfully to {ToEmail} with subject '{Subject}'", toEmail, subject);
        }
        catch (Exception ex)
        {
            // In development or when SMTP server is unreachable, log the email body
            _logger.LogWarning(ex, "Failed to send email via SMTP to {ToEmail}. Logging email content instead: {Subject} - {Body}",
                toEmail, subject, htmlBody);
        }
    }
}
