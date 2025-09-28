using System.Text;

namespace Payments.Application.Exceptions;

public class EntityStateInfo
{
    private readonly string _entityName;
    private readonly Dictionary<string, string> _properties;

    public EntityStateInfo(string entityName, params IEnumerable<(string PropertyName, string PropertyValue)> properties)
    {
        _entityName = entityName;
        _properties = properties.ToDictionary(
            x => x.PropertyName, 
            x => x.PropertyValue);
    }

    public override string ToString()
    {
        var properties = string.Join(", ",
            _properties.Select(x => $"{x.Key} = {x.Value}"));
        return new StringBuilder()
            .Append($"{_entityName} {{ {properties} }}")
            .ToString();
    }
}