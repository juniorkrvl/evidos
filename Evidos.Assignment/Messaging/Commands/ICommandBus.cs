namespace Evidos.Assignment.Messaging.Commands
{
    public interface ICommandBus
    {
        public Task DispatchAsync<TCommand>(TCommand command) where TCommand : ICommand;
    }
}
