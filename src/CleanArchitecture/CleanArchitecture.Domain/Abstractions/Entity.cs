namespace CleanArchitecture.Domain.Abstractions;

public abstract class Entity<TEntityId> : IEntity
{
    public Entity()
    {

    }
    private readonly List<IDomainEvent> _domainEvents = new();
    protected Entity(TEntityId id)
    {
        Id = id;
    }
    public TEntityId Id { get; init; } = default!;

    public IReadOnlyList<IDomainEvent> GetDomainEvents()
    {
        return _domainEvents.ToList();
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

}
