namespace CodeWay.Domain.Common;

/// <summary>
/// Abstract root for all domain entities.
/// Holds the primary key and a collection of domain events raised during
/// the entity's lifecycle. Events are dispatched post-save via an EF Core interceptor.
/// </summary>
public abstract class BaseEntity
{
    /// <summary>Surrogate primary key.</summary>
    public Guid Id { get; protected set; } = Guid.NewGuid();

    private readonly List<IDomainEvent> _domainEvents = [];

    /// <summary>Read-only snapshot of pending domain events.</summary>
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    /// <summary>Raise a domain event to be dispatched after the unit of work commits.</summary>
    protected void RaiseDomainEvent(IDomainEvent domainEvent) =>
        _domainEvents.Add(domainEvent);

    /// <summary>Called by the EF Core interceptor after events have been dispatched.</summary>
    public void ClearDomainEvents() => _domainEvents.Clear();
}
