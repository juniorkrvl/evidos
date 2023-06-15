using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Users
{
    internal class UserEventLifecycle : 
        IEventHandler<UserCreated>, 
        IEventHandler<UserEmailUpdated>,
        IEventHandler<UserDeleted>,
        IEventHandler<EmailChangeRequested>
    {
        private readonly IUserProjectionRepository _userReadRepository;
        private readonly IUserPrivateDataRepository _privateDataRepository;
        private readonly IPendingEmailChangeRepository _pendingEmailChangeRepository;

        public UserEventLifecycle(
            IUserProjectionRepository userReadRepository, 
            IUserPrivateDataRepository privateDataRepository,
            IPendingEmailChangeRepository pendingEmailChangeRepository)
        {
            _userReadRepository = userReadRepository;
            _privateDataRepository = privateDataRepository;
            _pendingEmailChangeRepository = pendingEmailChangeRepository;
        }

        public async Task Handle(UserCreated @event)
        {
            Console.WriteLine($"Event received (UserCreated)-> {@event.UserId} {@event.CreatedAt.ToString()}");

            var userData = await _privateDataRepository.GetByUserId(@event.UserId);
            if ( userData != null )
            {
                // Create a projection of the user
                var user = new UserViewModel(
                    @event.UserId, 
                    userData.Name,
                    userData.Email,
                    userData.Address
                );
                await _userReadRepository.Save(user);
            }
        }

        public async Task Handle(UserEmailUpdated @event)
        {
            Console.WriteLine($"Event received (UserEmailUpdated) -> {@event.UserId} {@event.UpdatedAt.ToString()}");

            var userData = await _privateDataRepository.GetByUserId(@event.UserId);
            if( userData != null )
            {
                var projection = await _userReadRepository.GetById(@event.UserId);
                if ( projection != null )
                {
                    projection.UpdateEmail(userData.Email, @event.UpdatedAt);
                    await _userReadRepository.Update(projection);
                }
            }
        }

        public async Task Handle(UserDeleted @event)
        {
            Console.WriteLine($"Event received (UserDeleted) -> {@event.UserId} {@event.DeletedAt.ToString()}");

            await _userReadRepository.Delete(@event.UserId);
        }

        public async Task Handle(EmailChangeRequested @event)
        {
            // TODO: send email to user
            var pendingRequest = await _pendingEmailChangeRepository.GetByToken(@event.Token);
            if (pendingRequest!=null)
            {
                Console.WriteLine($"Notification sent to user in the email: {pendingRequest.NewEmail}");    
            }
        }
    }
}
