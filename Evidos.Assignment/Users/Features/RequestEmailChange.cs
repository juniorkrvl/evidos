using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Persistence;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Users.Features
{
    public record RequestEmailChangeCommand(UserId UserId, Email NewEmail) : ICommand
    { }

    internal class RequestEmailChangeCommandHandler : ICommandHandler<RequestEmailChangeCommand>
    {
        private readonly IPendingEmailChangeRepository _pendingEmailChangeRequests;
        private readonly IUserPrivateDataRepository _privateData;
        private readonly IEventStore _eventStore;
        public RequestEmailChangeCommandHandler(
            IPendingEmailChangeRepository requestChangeRepository,
            IUserPrivateDataRepository userPrivateDataRepository,
            IEventStore eventStore
        )
        {
            _pendingEmailChangeRequests = requestChangeRepository;
            _privateData = userPrivateDataRepository;
            _eventStore = eventStore;
        }

        public async Task HandleAsync(RequestEmailChangeCommand command)
        {
            var user = await _privateData.GetByUserId(command.UserId);
            
            // Check if user exists and the email is not the same
            if (user == null) throw CannotRetrieveUser.BecauseUserWasNotFound(command.UserId);
            if (user.Email.Equals(command.NewEmail)) throw CannotChangeEmail.becauseTheEmailYouAreTryngToChangeIsAlreadyTheCurrentEmail(command.NewEmail);
            
            // Create a new pending email request
            var pendingChange = new PendingEmailChange(EmailChangeToken.Generate(), command.UserId, command.NewEmail);
            await _pendingEmailChangeRequests.Save(pendingChange);
            
            // Create the event, store and dispatch it
            var @event = new EmailChangeRequested(command.UserId, pendingChange.Token, DateTime.UtcNow);
            await _eventStore.Save(command.UserId.ToString(), new IEvent[] { @event });
        }
    }
    
    public record EmailChangeRequested(
        UserId UserId,
        EmailChangeToken Token,
        DateTime RequestedAt
    ) : IEvent
    {
        public UserId UserId { get; set; } = UserId;
        public EmailChangeToken Token { get; set; } = Token;
        public DateTime RequestedAt { get; set; } = RequestedAt;
    }
}
