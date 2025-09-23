namespace Payments.Application.Abstractions;

public interface ICommandSender
{
    public Task SendAsync(ICommand command, CancellationToken cancellationToken = default);
}