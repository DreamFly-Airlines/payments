using Payments.Application.Exceptions;

namespace Payments.Application.Helpers;

public static class DataParser
{
    public static T TryParseEnumOrThrow<T>(string value, string parameterName)
        where T : struct, Enum
    {
        if (!Enum.TryParse<T>(value, out var parsed))
        {
            var state = new EntityStateInfo(nameof(T), ("Value", value));
            throw new ValidationException(
                $"Unknown {parameterName}. " +
                $"Supported values: {string.Join(", ", Enum.GetNames<T>())}", 
                state);
        }
        return parsed;
    }
}