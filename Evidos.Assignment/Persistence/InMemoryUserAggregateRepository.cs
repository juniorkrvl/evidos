using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Persistence
{
    internal class InMemoryUserAggregateRepository : IUserAggregateRepository
    {
        private readonly IEventStore _eventStore;

        public InMemoryUserAggregateRepository(IEventStore eventStore, IEventBus eventBus)
        {
            _eventStore = eventStore;
        }

        public async Task<User> GetById(UserId id)
        {
            var events = await _eventStore.GetById(id.ToString());
            return new User(events);
        }

        public Task Save(User user)
        {
            _eventStore.Save(user.Id.ToString(), user.DomainEvents);
            return Task.CompletedTask;
        }
    }

}