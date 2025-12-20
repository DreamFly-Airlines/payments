using Payments.Domain.Exceptions;

namespace Payments.Domain.ValueObjects;

public readonly record struct Money
{
    public decimal Amount { get; }
    public Currency Currency { get; }

    public Money(decimal amount, Currency currency)
    {
        if (amount < 0)
            throw new InvalidDomainDataFormatException("Money amount should be greater than or equal to 0");

        if (decimal.Round(amount, currency.IsoMinorUnit) != amount)
            throw new InvalidDomainOperationException(
                $"Amount \"{amount}\" has more decimal places than allowed for currency {currency.IsoCode}. " +
                $"Max {currency.IsoMinorUnit} decimals");
        
        Amount = amount;
        Currency = currency;
    }
}