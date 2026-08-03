using MediatR;

namespace CodeWay.Domain.Common;

/// <summary>
/// Marker interface for all domain events.
/// Implements <see cref="INotification"/> so MediatR can dispatch events
/// to their respective handlers without coupling the domain to infrastructure.
/// </summary>
public interface IDomainEvent : INotification
{
    /// <summary>Moment the event occurred (UTC).</summary>
    DateTime OccurredOnUtc { get; }
}
