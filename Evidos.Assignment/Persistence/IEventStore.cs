using Evidos.Assignment.Messaging.Events;

namespace Evidos.Assignment.Persistence
{
    public interface IEventStore
    {
        Task Save(string aggregateId, IEnumerable<IEvent> events);
        Task<IEnumerable<IEvent>> GetById(string aggregateId);
    }
}
