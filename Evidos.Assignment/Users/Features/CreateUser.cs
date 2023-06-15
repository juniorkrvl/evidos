using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Users.Features
{
    public record CreateUserCommand(string Name, Email Email, string Address) : ICommand
    { }

    internal class CreateUserCommandHandler : ICommandHandler<CreateUserCommand>
    {
        private readonly IUserAggregateRepository _userAggregateRepository;
        private readonly IUserPrivateDataRepository _userPrivateDataRepository;
        public CreateUserCommandHandler(IUserAggregateRepository userRepository, IUserPrivateDataRepository userPrivateDataRepository)
        {
            _userAggregateRepository = userRepository;
            _userPrivateDataRepository = userPrivateDataRepository;
        }

        public async Task HandleAsync(CreateUserCommand command)
        {
            // Check uniqueness of email before creating user
            var emailExists = await _userPrivateDataRepository.GetByEmail(command.Email);
            if (emailExists != null)
            {
                throw CannotCreateUser.becauseEmailAlreadyExists(command.Email.ToString());
            }

            // Save User PII data 
            var userId = UserId.Generate();
            var userData = new UserData(userId, command.Name, command.Email, command.Address);
            await _userPrivateDataRepository.Save(userData);

            // Create aggregate passing the private data token
            var user = new User(userId);
            await _userAggregateRepository.Save(user);
        }
    }

    public record UserCreated(
        UserId UserId,
        DateTime CreatedAt
    ) : IEvent
    {
        public UserId UserId { get; set; } = UserId;
        public DateTime CreatedAt { get; set; } = CreatedAt;
    }




}
