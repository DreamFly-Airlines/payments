namespace Payments.Domain.Exceptions;

public class DomainModelCreationException(string message) : Exception(message);