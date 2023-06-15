using Evidos.Assignment.Messaging.Events;

namespace Evidos.Assignment.Aggregate
{
    public abstract class Aggregate<TIdentity> where TIdentity : IAggregateId
    {
        public abstract TIdentity Id { get; protected set; }

        private readonly List<IEvent> _domainEvents = new();
        public IReadOnlyCollection<IEvent> DomainEvents => _domainEvents.AsReadOnly();

        private int Version { get; set; }

        protected Aggregate()
        {
        }

        protected Aggregate(IEnumerable<IEvent> events)
        {
            foreach (var @event in events)
            {
                Mutate(@event);
                Version++;
            }
        }

        protected void Apply(IEnumerable<IEvent> events)
        {
            foreach (IEvent @event in events)
            {
                Apply(@event);
            }
        }

        protected void Apply(IEvent @event)
        {
            Mutate(@event);
            AddEvent(@event);
        }

        private void Mutate(IEvent @event)
        {
            ((dynamic)this).On((dynamic)@event);
        }

        private void AddEvent(IEvent @event)
        {
            _domainEvents.Add(@event);
        }

        protected void RemoveEvent(IEvent @event)
        {
            _domainEvents.Remove(@event);
        }

        protected void FLushEvents()
        {
            _domainEvents.Clear();
        }
    }
}
