namespace Payments.Application.Exceptions;

public class NotFoundException(string className, string id) : Exception($"{className} with ID \"{id}\" not found.");