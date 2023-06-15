using Evidos.Assignment.Users.Models;

namespace Evidos.Assignment.Users.Repositories
{
    internal interface IPendingEmailChangeRepository
    {
        Task<PendingEmailChange?> GetByToken(EmailChangeToken token);
        Task Delete(EmailChangeToken token);
        Task DeleteByUserId(UserId userId);
        Task Save(PendingEmailChange entity);
        Task<IEnumerable<PendingEmailChange>> GetAll();
    }
}
