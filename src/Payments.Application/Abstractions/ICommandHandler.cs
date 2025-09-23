namespace Payments.Application.Abstractions;

public interface ICommandHandler<TCommand> where TCommand : ICommand
{
    public Task HandleAsync(CancellationToken cancellationToken = default);
}