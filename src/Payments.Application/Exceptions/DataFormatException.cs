namespace Payments.Application.Exceptions;

public class DataFormatException(string paramName, string? value, string reason) 
    : Exception($"Invalid value for {paramName}{(value is null ? "" : $": \"{value}\"")}. Reason: {reason}.");