using Payments.Domain.Exceptions;

namespace Payments.Domain.ValueObjects;

public readonly record struct Currency
{
    private static readonly Dictionary<string, Currency> IsoCodeToCurrency =
        new List<Currency> 
            {
                new("RUB", 2)
            }
            .ToDictionary(c => c.IsoCode);
    
    public string IsoCode { get; }
    public int MinorUnitCount { get; }

    private Currency(string isoCode, int minorUnitCount)
    {
        IsoCode = isoCode;
        MinorUnitCount = minorUnitCount;
    }

    public static Currency FromIsoString(string isoCode)
    {
        if (!IsoCodeToCurrency.TryGetValue(isoCode, out var currency))
            throw new InvalidDomainDataFormatException(
                $"Unsupported iso currency code. Supported are: {string.Join(", ", IsoCodeToCurrency.Keys)}");

        return currency;
    }
}