using Evidos.Assignment.Messaging.Queries;
using Evidos.Assignment.Users.Models;
using Evidos.Assignment.Users.Repositories;

namespace Evidos.Assignment.Users.Features
{
    public record GetPendingEmailRequestsQuery() : IQuery<IEnumerable<PendingEmailChangeDto>>
    { }

    internal class GetPendingEmailRequestsQueryHandler : IQueryHandler<GetPendingEmailRequestsQuery, IEnumerable<PendingEmailChangeDto>>
    {
        private readonly IPendingEmailChangeRepository _pendingEmailChange;

        public GetPendingEmailRequestsQueryHandler(IPendingEmailChangeRepository pendingEmailChange)
        {
            _pendingEmailChange = pendingEmailChange;
        }

        public Task<IEnumerable<PendingEmailChangeDto>> HandleAsync(GetPendingEmailRequestsQuery query)
        {
            return Task.FromResult(_pendingEmailChange.GetAll().Result.Select(
                    PendingEmailChangeDto.FromModel
                )
            );
        }
    }
}