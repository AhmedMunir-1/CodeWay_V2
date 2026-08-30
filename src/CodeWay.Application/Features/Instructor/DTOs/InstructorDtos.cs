namespace CodeWay.Application.Features.Instructor.DTOs;

using CodeWay.Domain.Enums;

public class InstructorProfileDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Headline { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public string PayoutEmail { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreateInstructorProfileDto
{
    public string Headline { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public string PayoutEmail { get; set; } = string.Empty;
}

public class UpdateInstructorProfileDto
{
    public string Headline { get; set; } = string.Empty;
    public string Biography { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public string? TwitterUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? YouTubeUrl { get; set; }
    public string PayoutEmail { get; set; } = string.Empty;
}

public class InstructorWalletDto
{
    public Guid Id { get; set; }
    public Guid InstructorId { get; set; }
    public decimal Balance { get; set; }
    public decimal PendingBalance { get; set; }
    public decimal TotalEarned { get; set; }
    public List<WalletTransactionDto> RecentTransactions { get; set; } = [];
}

public class WalletTransactionDto
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public decimal Amount { get; set; }
    public WalletTransactionType Type { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ReferenceId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class PayoutRequestDto
{
    public Guid Id { get; set; }
    public Guid InstructorId { get; set; }
    public string InstructorName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public PayoutStatus Status { get; set; }
    public string PayoutMethod { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public DateTime? ProcessedAtUtc { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public class CreatePayoutRequestDto
{
    public decimal Amount { get; set; }
    public string PayoutMethod { get; set; } = "PayPal";
    public string? Notes { get; set; }
}

public class ProcessPayoutRequestDto
{
    public PayoutStatus Status { get; set; }
    public string? Notes { get; set; }
}
