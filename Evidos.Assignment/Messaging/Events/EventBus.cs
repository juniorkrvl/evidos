using System.Collections;
using Microsoft.Extensions.DependencyInjection;

namespace Evidos.Assignment.Messaging.Events
{
    public class EventBus : IEventBus
    {
        private readonly IServiceProvider _serviceProvider;

        public EventBus(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
        {
            var eventType = @event.GetType();
            var handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
            var handlers = (IEnumerable)_serviceProvider.GetServices(handlerType);

            foreach (dynamic handler in handlers)
            {                
                await handler.Handle((dynamic)@event);
            }
        }

    }
}
