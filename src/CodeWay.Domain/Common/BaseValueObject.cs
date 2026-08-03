namespace CodeWay.Domain.Common;

/// <summary>
/// Abstract base for all Value Objects.
/// Value objects have no identity — they are equal when all their
/// components are equal. Override <see cref="GetEqualityComponents"/>
/// to define what constitutes equality.
/// </summary>
public abstract class BaseValueObject : IEquatable<BaseValueObject>
{
    protected abstract IEnumerable<object?> GetEqualityComponents();

    public bool Equals(BaseValueObject? other) =>
        other is not null && GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());

    public override bool Equals(object? obj) =>
        obj is BaseValueObject other && Equals(other);

    public override int GetHashCode() =>
        GetEqualityComponents()
            .Select(x => x?.GetHashCode() ?? 0)
            .Aggregate((x, y) => x ^ y);

    public static bool operator ==(BaseValueObject? left, BaseValueObject? right) =>
        left?.Equals(right) ?? right is null;

    public static bool operator !=(BaseValueObject? left, BaseValueObject? right) =>
        !(left == right);
}
