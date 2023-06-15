using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users.Repositories
{
    public interface IUserProjectionRepository
    {
        Task<UserViewModel?> GetById(UserId id);
        Task<UserViewModel?> GetByEmail(Email email);
        Task<IEnumerable<UserViewModel>> GetAllAsync();
        Task<IEnumerable<UserViewModel>> FindByFullOrPartialEmail(string email);
        Task Save(UserViewModel entity);
        Task Update(UserViewModel entity);
        Task Delete(UserId id);
    }
}
