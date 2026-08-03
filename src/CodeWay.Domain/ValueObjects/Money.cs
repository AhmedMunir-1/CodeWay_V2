namespace CodeWay.Domain.ValueObjects;

using CodeWay.Domain.Common;
using CodeWay.Domain.Exceptions;

/// <summary>
/// Represents a monetary amount with an ISO 4217 currency code.
/// Prevents floating-point rounding errors by using <see cref="decimal"/>.
/// </summary>
public sealed class Money : BaseValueObject
{
    public decimal Amount { get; }
    public string Currency { get; }  // ISO 4217 e.g. "USD"

    private Money() { }  // EF Core ctor

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency.ToUpperInvariant();
    }

    public static Money Of(decimal amount, string currency)
    {
        if (amount < 0)
            throw new DomainException("Money amount cannot be negative.");

        if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
            throw new DomainException("Currency must be a 3-letter ISO 4217 code.");

        return new Money(amount, currency);
    }

    public static Money Zero(string currency = "USD") => new(0m, currency);

    public Money Add(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot add {Currency} and {other.Currency}.");
        return new Money(Amount + other.Amount, Currency);
    }

    public Money Subtract(Money other)
    {
        if (Currency != other.Currency)
            throw new DomainException($"Cannot subtract {other.Currency} from {Currency}.");
        if (Amount < other.Amount)
            throw new DomainException("Insufficient funds.");
        return new Money(Amount - other.Amount, Currency);
    }

    public override string ToString() => $"{Amount:F2} {Currency}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Amount;
        yield return Currency;
    }
}
