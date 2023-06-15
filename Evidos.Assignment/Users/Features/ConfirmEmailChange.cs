using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Persistence;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Users.Features
{
    public record ConfirmEmailChange(EmailChangeToken Token) : ICommand
    { }

    internal class ConfirmEmailChangeCommandHandler : ICommandHandler<ConfirmEmailChange>
    {
        private readonly IPendingEmailChangeRepository _pendingRequests;
        private readonly IUserPrivateDataRepository _privateData;
        private readonly IEventStore _eventStore;
        public ConfirmEmailChangeCommandHandler(
            IPendingEmailChangeRepository pendingRequests,
            IUserPrivateDataRepository privateData,
            IEventStore eventStore
        )
        {
            _pendingRequests = pendingRequests;
            _privateData = privateData;
            _eventStore = eventStore;
        }
        public async Task HandleAsync(ConfirmEmailChange command)
        {
            // Fetch pending email change request
            var pendingRequest = await _pendingRequests.GetByToken(command.Token);
            if (pendingRequest == null) throw CannotChangeEmail.becauseThereIsNoPendingEmailChangeRequest(command.Token);

            // Update email in the private data repository
            var userData = await _privateData.GetByUserId(pendingRequest.UserId);
            if (userData == null) throw CannotRetrieveUser.BecausePrivateDataIsNotAvailable(pendingRequest.UserId);

            userData.UpdateEmail(pendingRequest.NewEmail);      

            await Task.WhenAll(
                _privateData.Update(userData),
                _pendingRequests.Delete(command.Token)
            );

            // Stores and dispatches the event
            var @event = new UserEmailUpdated(pendingRequest.UserId, DateTime.UtcNow);
            await _eventStore.Save(userData.UserId.ToString(), new []{@event} );
        }
    }
    
    public record UserEmailUpdated(
        UserId UserId,
        DateTime UpdatedAt
    ) : IEvent
    {
        public UserId UserId { get; set; } = UserId;
        public DateTime UpdatedAt { get; set; } = UpdatedAt;
    }
}
