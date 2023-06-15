using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Evidos.Assignment.Messaging.Commands;
using Evidos.Assignment.Messaging.Events;
using Evidos.Assignment.Messaging.Queries;
using Evidos.Assignment.Persistence;
using Evidos.Assignment.Users.Features;
using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users.Services
{
    public class UserService
    {
        private readonly ICommandBus _commandBus;
        private readonly IQueryBus _queryBus;
        private readonly IEventStore _eventStore;
        public UserService(ICommandBus commandBus, IQueryBus queryBus, IEventStore eventStore)
        {
            _commandBus = commandBus;
            _queryBus = queryBus;
            _eventStore = eventStore;
        }

        public async Task CreateUser(string name, string email, string address)
        {
            await _commandBus.DispatchAsync(new CreateUserCommand(name, Email.FromString(email), address));
        }
        
        public async Task DeleteUser(string userId)
        {
            await _commandBus.DispatchAsync(new DeleteUserCommand(new UserId(userId)));
        }

        public async Task RequestEmailChange(string userId, Email email)
        {
            await _commandBus.DispatchAsync(new RequestEmailChangeCommand(new UserId(userId), email));
        }

        public async Task ConfirmEmailChange(string token)
        {
            await _commandBus.DispatchAsync(new ConfirmEmailChange(new EmailChangeToken(token)));
        }

        public Task<UserDto> GetUserByEmail(string email)
        {
            return _queryBus.DispatchAsync<GetUserByEmailQuery, UserDto>(new GetUserByEmailQuery(Email.FromString(email)));
        }

        public Task<IEnumerable<UserDto>> SearchUserByEmail(string email)
        {
            return _queryBus.DispatchAsync<SearchUsersByFullOrPartialEmailQuery, IEnumerable<UserDto>>(new SearchUsersByFullOrPartialEmailQuery(email));
        }

        public Task<IEnumerable<IEvent>> GetEvents(string id)
        {
            return _eventStore.GetById(id);
        }
        
        public Task<IEnumerable<PendingEmailChangeDto>> GetPendingEmailChanges()
        {
            return _queryBus.DispatchAsync<GetPendingEmailRequestsQuery, IEnumerable<PendingEmailChangeDto>>(new GetPendingEmailRequestsQuery());
        }
        
    }
}
