using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Persistence
{
    internal class InMemoryUserPrivateDataRepository : IUserPrivateDataRepository
    {
        private readonly Dictionary<UserId, UserData> _storage = new();
        public Task Save(UserData entity)
        {
            _storage[entity.UserId] = entity;
            return Task.CompletedTask;
        }

        public Task Delete(UserId token)
        {
            _storage.Remove(token);
            return Task.CompletedTask;
        }

        public Task<IEnumerable<UserData>> GetAllAsync()
        {
            return Task.FromResult(_storage.Values.AsEnumerable());
        }

        public Task<UserData?> GetByEmail(Email email)
        {
            var user = _storage.Values.FirstOrDefault(x => x.Email.Equals(email));
            return Task.FromResult(user);
        }

        public Task<UserData?> GetByUserId(UserId userId)
        {
            var user = _storage.Values.FirstOrDefault(x => x.UserId.Equals(userId));
            return Task.FromResult(user);
        }

        public Task<UserData?> GetByTokenAsync(UserId id)
        {
            _storage.TryGetValue(id, out var user);
            return Task.FromResult(user);
        }

        public Task Update(UserData entity)
        {
            if (_storage.ContainsKey(entity.UserId))
            {
                _storage[entity.UserId] = entity;
            }

            return Task.CompletedTask;
        }
    }
}
