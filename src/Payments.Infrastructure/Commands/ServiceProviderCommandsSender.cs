using Microsoft.Extensions.DependencyInjection;
using Payments.Application.Abstractions;

namespace Payments.Infrastructure.Commands;

public class ServiceProviderCommandSender(
    IServiceProvider serviceProvider) : ICommandSender
{
    private const string HandleAsyncMethodName = nameof(ICommandHandler<ICommand>.HandleAsync);

    public async Task SendAsync(ICommand command, CancellationToken cancellationToken = default)
    {
        var commandType = command.GetType();
        var commandHandlerType = typeof(ICommandHandler<>).MakeGenericType(commandType);
        var commandHandler = serviceProvider.GetRequiredService(commandHandlerType);
        var resultTaskObject = commandHandlerType
                                   .GetMethod(HandleAsyncMethodName)?
                                   .Invoke(commandHandler, [command, cancellationToken]) 
                               ?? throw new MethodAccessException(
                                   $"Cannot find or invoke method \"{HandleAsyncMethodName}\" " +
                                   $"of {commandHandlerType.Name}.");
        await (Task)resultTaskObject;
    }
}