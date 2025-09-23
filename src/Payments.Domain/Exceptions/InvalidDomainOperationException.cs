namespace Payments.Domain.Exceptions;

public class InvalidDomainOperationException(string message) : Exception(message);