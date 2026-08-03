namespace CodeWay.Domain.Entities.Learning;

using CodeWay.Domain.Common;

public class Certificate : AuditableEntity
{
    public Guid EnrollmentId { get; set; }
    public string CertificateCode { get; set; } = string.Empty;
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;
    public string PdfUrl { get; set; } = string.Empty;

    // Navigation properties
    public Enrollment Enrollment { get; set; } = null!;
}
