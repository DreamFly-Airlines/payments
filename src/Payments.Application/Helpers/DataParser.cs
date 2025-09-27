using Payments.Application.Exceptions;

namespace Payments.Application.Helpers;

public static class DataParser
{
    public static T TryParseEnumOrThrow<T>(string value, string parameterName)
        where T : struct, Enum
    {
        if (!Enum.TryParse<T>(value, out var parsed))
            throw new DataFormatException(
                parameterName, value, $"Supported values: {string.Join(", ", Enum.GetNames<T>())}.");
        return parsed;
    }
}