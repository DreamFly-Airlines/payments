using Payments.Domain.Exceptions;

namespace Payments.Domain.ValueObjects;

public readonly record struct Currency
{
    private static readonly Dictionary<string, Currency> SupportedIsoCodes = new()
    {
        [Rub.IsoCode] = Rub
    };

    public string IsoCode { get; private init; }
    public int MinorUnitCount { get; private init; }

    public static Currency Rub => new() { IsoCode = "RUB", MinorUnitCount = 2 };

    public static Currency FromIsoString(string isoCode)
    {
        if (!SupportedIsoCodes.TryGetValue(isoCode, out var currency))
            throw new InvalidDomainDataFormatException(
                $"Unsupported iso currency code. Supported are: {string.Join(", ", SupportedIsoCodes.Keys)}");

        return currency;
    }
}