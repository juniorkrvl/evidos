using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Persistence;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Users.Features
{
    public record DeleteUserCommand(UserId UserId) : ICommand
    { }

    internal class DeleteUserCommandHandler : ICommandHandler<DeleteUserCommand>
    {
        private readonly IUserPrivateDataRepository _privateData;
        private readonly IPendingEmailChangeRepository _pendingEmailChangeRepository;
        private readonly IEventStore _eventStore;

        public DeleteUserCommandHandler(
            IEventStore eventStore, 
            IUserPrivateDataRepository privateData,
            IPendingEmailChangeRepository pendingEmailChangeRepository
        )
        {
            _eventStore = eventStore;
            _privateData = privateData;
            _pendingEmailChangeRepository = pendingEmailChangeRepository;
        }

        public async Task HandleAsync(DeleteUserCommand command)
        {
            var userData = await _privateData.GetByUserId(command.UserId);
            if (userData == null) throw CannotRetrieveUser.BecausePrivateDataIsNotAvailable(command.UserId);

            Task.WaitAll(
                _pendingEmailChangeRepository.DeleteByUserId(userData.UserId),
                _privateData.Delete(userData.UserId)
            );
            
            var @event = new UserDeleted(command.UserId, DateTime.UtcNow);
            await _eventStore.Save(userData.UserId.ToString(), new IEvent[] { @event });
        }

    }


    public record UserDeleted(
        UserId UserId,
        DateTime DeletedAt
    ) : IEvent
    {
        public UserId UserId { get; set; } = UserId;
        public DateTime DeletedAt { get; set; } = DeletedAt;
    }
}
