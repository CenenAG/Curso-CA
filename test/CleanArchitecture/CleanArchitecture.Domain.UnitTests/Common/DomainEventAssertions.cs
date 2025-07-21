using CleanArchitecture.Domain.Abstractions;
using FluentAssertions;

namespace CleanArchitecture.Domain.UnitTests.Common;

public static class DomainEventAssertions
{
    public static void AssertDomainEventWasPublished<TDomainEvent, TEntityId>(Entity<TEntityId> entity)
        where TDomainEvent : class, IDomainEvent
    {
        var domainEvents = entity.GetDomainEvents();

        domainEvents.Should().NotBeEmpty();
        domainEvents.Should().ContainSingle(e => e is TDomainEvent);
    }

    public static void AssertDomainEventWasPublished<TDomainEvent, TEntityId>(Entity<TEntityId> entity, int expectedCount)
        where TDomainEvent : class, IDomainEvent
    {
        var domainEvents = entity.GetDomainEvents();

        domainEvents.Should().NotBeEmpty();
        domainEvents.Should().Contain(e => e is TDomainEvent).And.HaveCount(expectedCount);
    }

    public static TDomainEvent GetPublishedDomainEvent<TDomainEvent, TEntityId>(Entity<TEntityId> entity)
        where TDomainEvent : class, IDomainEvent
    {
        var domainEvents = entity.GetDomainEvents();

        domainEvents.Should().NotBeEmpty();
        domainEvents.Should().ContainSingle(e => e is TDomainEvent);

        var domainEvent = domainEvents.First(e => e is TDomainEvent) as TDomainEvent;
        domainEvent.Should().NotBeNull();

        return domainEvent!;
    }

    public static void AssertNoDomainEventsWerePublished<TEntityId>(Entity<TEntityId> entity)
    {
        var domainEvents = entity.GetDomainEvents();
        domainEvents.Should().BeEmpty();
    }

    public static void AssertDomainEventsCount<TEntityId>(Entity<TEntityId> entity, int expectedCount)
    {
        var domainEvents = entity.GetDomainEvents();
        domainEvents.Should().HaveCount(expectedCount);
    }

    public static void AssertDomainEventOfType<TDomainEvent, TEntityId>(Entity<TEntityId> entity, Action<TDomainEvent> assertion)
        where TDomainEvent : class, IDomainEvent
    {
        var domainEvent = GetPublishedDomainEvent<TDomainEvent, TEntityId>(entity);
        assertion(domainEvent);
    }
}