using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Persistence
{
    internal class InMemoryUserProjectionRepository : IUserProjectionRepository
    {
        private readonly Dictionary<UserId, UserViewModel> _storage = new();

        public Task<UserViewModel?> GetById(UserId id)
        {
            _storage.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task<UserViewModel?> GetByEmail(Email email)
        {
            var user = _storage.Values.FirstOrDefault(x => x.Email.Equals(email));
            return Task.FromResult(user);
        }

        public Task<IEnumerable<UserViewModel>> GetAllAsync()
        {
            return Task.FromResult(_storage.Values.AsEnumerable());
        }

        public Task Save(UserViewModel entity)
        {
            _storage[entity.Id] = entity;
            return Task.CompletedTask;
        }

        public Task Update(UserViewModel entity)
        {
            if (_storage.ContainsKey(entity.Id))
            {
                _storage[entity.Id] = entity;
            }

            return Task.CompletedTask;
        }

        public Task Delete(UserId id)
        {
            _storage.Remove(id);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserViewModel>> FindByFullOrPartialEmail(string email)
        {
            var users = _storage.Values.Where(x => x.Email.ToString().Contains(email));
            return Task.FromResult(users);
        }
    }

}
