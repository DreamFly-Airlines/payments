namespace Payments.Application.Exceptions;

public class ValidationException(string message, EntityStateInfo? entityStateInfo = null) : Exception(message)
{
    public EntityStateInfo? EntityStateInfo { get; init; } = entityStateInfo;
}