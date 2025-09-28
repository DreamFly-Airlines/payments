using Payments.Domain.Exceptions;

namespace Payments.Domain.ValueObjects;

public readonly record struct LastFour
{
    private readonly string _value;
    
    private LastFour(string value) => _value = value;

    public static LastFour FromString(string value)
    {
        if (value.Length != 4) 
            throw new InvalidDomainDataFormatException("Last four should consist only of 4 digits.");
        return new(value);
    }
    
    public override string ToString() => _value;

    public static implicit operator string(LastFour lastFour) => lastFour._value;
}