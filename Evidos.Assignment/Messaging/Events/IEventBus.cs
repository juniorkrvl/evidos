namespace Evidos.Assignment.Messaging.Events
{
    public interface IEventBus
    {
        public Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;
    }
}
