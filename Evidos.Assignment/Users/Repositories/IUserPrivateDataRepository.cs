using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users.Repositories
{
    internal interface IUserPrivateDataRepository
    {
        Task<UserData?> GetByEmail(Email email);
        Task<UserData?> GetByUserId(UserId userId);
        Task Save(UserData entity);
        Task Update(UserData entity);
        Task Delete(UserId token);

    }
}
