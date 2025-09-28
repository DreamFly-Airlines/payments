namespace Payments.Domain.Exceptions;

public class InvalidDomainDataFormatException(string message) : Exception(message);