using Evidos.Assignment.Messaging.Events;

namespace Evidos.Assignment.Persistence
{
    internal class InMemoryEventStore : IEventStore
    {
        private readonly Dictionary<string, List<IEvent>> _store = new Dictionary<string, List<IEvent>>();

        private readonly IEventBus _eventBus;
        public InMemoryEventStore(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public Task Save(string aggregateId, IEnumerable<IEvent> events)
        {
            if (!_store.ContainsKey(aggregateId))
            {
                _store[aggregateId] = new List<IEvent>();
            }

            var collection = events as IEvent[] ?? events.ToArray();
            _store[aggregateId].AddRange(collection);
            foreach (var @event in collection)
            {
                _eventBus.Publish(@event);
            }
            
            return Task.CompletedTask;
        }

        public Task<IEnumerable<IEvent>> GetById(string aggregateId)
        {
            return Task.FromResult<IEnumerable<IEvent>>(_store.ContainsKey(aggregateId) ? _store[aggregateId] : new List<IEvent>());
        }

    }
}
