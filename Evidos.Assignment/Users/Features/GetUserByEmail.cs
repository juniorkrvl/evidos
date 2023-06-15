using Evidos.Assignment.Messaging.Queries;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Users.Features
{
    public record GetUserByEmailQuery(Email Email) : IQuery<UserDto>
    { }

    public class GetUserByEmailQueryHandler : IQueryHandler<GetUserByEmailQuery, UserDto>
    {
        private readonly IUserProjectionRepository _userRepository;

        public GetUserByEmailQueryHandler(IUserProjectionRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> HandleAsync(GetUserByEmailQuery query)
        {
            var user = await _userRepository.GetByEmail(query.Email);
            if (user == null) throw CannotRetrieveUser.BecauseUserWasNotFound(query.Email);
            return UserDto.FromUserViewModel(user);
        }
    }
}
