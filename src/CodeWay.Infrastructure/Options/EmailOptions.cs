namespace CodeWay.Infrastructure.Options;

public sealed class EmailOptions
{
    public const string SectionName = "Email";

    public string From { get; set; } = "noreply@codeway.com";
    public string SmtpHost { get; set; } = "localhost";
    public int SmtpPort { get; set; } = 1025;
    public string? UserName { get; set; }
    public string? Password { get; set; }
    public bool UseSsl { get; set; } = false;
}
