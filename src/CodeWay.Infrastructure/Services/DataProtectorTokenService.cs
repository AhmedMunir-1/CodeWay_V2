namespace CodeWay.Infrastructure.Services;

using CodeWay.Application.Contracts;
using Microsoft.AspNetCore.DataProtection;

public class DataProtectorTokenService : IDataProtectorTokenService
{
    private readonly IDataProtectionProvider _dataProtectionProvider;

    public DataProtectorTokenService(IDataProtectionProvider dataProtectionProvider)
    {
        _dataProtectionProvider = dataProtectionProvider;
    }

    public string GenerateToken(string purpose, string payload)
    {
        var protector = _dataProtectionProvider.CreateProtector(purpose);
        // Include timestamp into protected payload for expiry check (e.g. 24 hours)
        var timedPayload = $"{payload}|{DateTime.UtcNow:O}";
        return protector.Protect(timedPayload);
    }

    public string? ValidateToken(string purpose, string token)
    {
        try
        {
            var protector = _dataProtectionProvider.CreateProtector(purpose);
            var unprotected = protector.Unprotect(token);
            var parts = unprotected.Split('|');
            if (parts.Length != 2)
            {
                return null;
            }

            var payload = parts[0];
            if (!DateTime.TryParse(parts[1], out var createdAtUtc))
            {
                return null;
            }

            // Valid for 24 hours
            if (DateTime.UtcNow - createdAtUtc > TimeSpan.FromHours(24))
            {
                return null;
            }

            return payload;
        }
        catch
        {
            return null;
        }
    }
}
