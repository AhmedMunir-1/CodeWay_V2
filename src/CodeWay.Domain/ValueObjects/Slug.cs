namespace CodeWay.Domain.ValueObjects;

using System.Text.RegularExpressions;
using CodeWay.Domain.Common;
using CodeWay.Domain.Exceptions;

/// <summary>
/// URL-safe identifier for courses (e.g. "intro-to-csharp-2024").
/// Slugs are lowercase, hyphen-separated, alphanumeric only.
/// </summary>
public sealed class Slug : BaseValueObject
{
    private static readonly Regex SlugRegex = new(
        @"^[a-z0-9]+(?:-[a-z0-9]+)*$",
        RegexOptions.Compiled);

    public string Value { get; }

    private Slug() { Value = null!; }  // EF Core ctor
    private Slug(string value) => Value = value;

    public static Slug Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException("Slug cannot be empty.");

        if (!SlugRegex.IsMatch(value))
            throw new DomainException($"'{value}' is not a valid slug. Use lowercase letters, numbers, and hyphens only.");

        return new Slug(value);
    }

    /// <summary>Generates a slug from a raw title string.</summary>
    public static Slug FromTitle(string title)
    {
        var slug = title.ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"\s+", "-");
        slug = slug.Trim('-');
        return Of(slug);
    }

    public static implicit operator string(Slug slug) => slug.Value;
    public override string ToString() => Value;

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Value;
    }
}
