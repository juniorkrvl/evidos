using Evidos.Assignment.Messaging.Queries;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Users.Features
{
    public record SearchUsersByFullOrPartialEmailQuery (string Email) : IQuery<IEnumerable<UserDto>>
    { }

    internal class SearchUsersByFullOrPartialEmailQueryHandler : IQueryHandler<SearchUsersByFullOrPartialEmailQuery, IEnumerable<UserDto>>
    {
        private readonly IUserProjectionRepository _userRepository;

        public SearchUsersByFullOrPartialEmailQueryHandler(IUserProjectionRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<IEnumerable<UserDto>> HandleAsync(SearchUsersByFullOrPartialEmailQuery query)
        {
            var users = await _userRepository.FindByFullOrPartialEmail(query.Email);
            return users.Select(UserDto.FromUserViewModel);
        }
    }
}
