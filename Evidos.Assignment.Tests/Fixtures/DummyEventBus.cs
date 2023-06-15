using Evidos.Assignment.Messaging.Events;

namespace Evidos.Assignment.Tests.Fixtures;

public class DummyEventBus: IEventBus
{
    private readonly List<IEvent> _events = new ();


    public Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        _events.Add(@event);
        return Task.CompletedTask;
    }

    public Task<List<IEvent>> DispatchedEvents()
    {
        return Task.FromResult(_events);
    }
}