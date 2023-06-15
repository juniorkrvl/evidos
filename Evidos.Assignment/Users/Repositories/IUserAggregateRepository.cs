using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users.Repositories
{
    public interface IUserAggregateRepository
    {
        Task<User> GetById(UserId id);

        Task Save(User user);
    }
}
