using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Persistence
{
    internal class InMemoryPendingEmailChangeRepository : IPendingEmailChangeRepository
    {
        private readonly Dictionary<EmailChangeToken, PendingEmailChange> _storage = new();

        public Task<PendingEmailChange?> GetByToken(EmailChangeToken token)
        {
            _storage.TryGetValue(token, out var user);
            return Task.FromResult(user);
        }

        public Task DeleteByUserId(UserId userId)
        {
            var results = _storage.Values.ToList();
            foreach (var result in results)
            {
                _storage.Remove(result.Token);
            }
            return Task.CompletedTask;
        }

        public Task Save(PendingEmailChange entity)
        {
            _storage[entity.Token] = entity;
            return Task.CompletedTask;
        }

        public Task<IEnumerable<PendingEmailChange>> GetAll()
        {
            return Task.FromResult<IEnumerable<PendingEmailChange>>(_storage.Values.ToList());
        }

        public Task Delete(EmailChangeToken token)
        {
            _storage.Remove(token);
            return Task.CompletedTask;
        }
        
    }

}
