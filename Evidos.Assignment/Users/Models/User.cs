using Evidos.Assignment.Aggregate;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Users.Features;

namespace Evidos.Assignment.Users.Models
{
    public sealed class User : Aggregate<UserId>
    {
        public override UserId Id { get; protected set; }

        public User(IEnumerable<IEvent> events) : base(events)
        { }

        public User(UserId userId)
        {
            var userCreated = new UserCreated(
                userId,
                DateTime.UtcNow
            );

            Apply(userCreated);
        }

        public void On(UserCreated userCreatedEvent)
        {
            Id = userCreatedEvent.UserId;
        }
    }
}
