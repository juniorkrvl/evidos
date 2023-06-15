namespace Evidos.Assignment.Messaging.Queries
{
    public interface IQueryBus
    {
        public Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>;
    }
}
