namespace Payments.Api.Helpers;

public static class EnumFromStringValidationHelper
{
    private static readonly Dictionary<Type, HashSet<string>> AllowedEnumValuesCache = new();
    
    public static HashSet<string> GetAllowedEnumValues<T>() where T : Enum
    {
        if (AllowedEnumValuesCache.TryGetValue(typeof(T), out var allowed))
            return allowed;
        var result = new HashSet<string>();
        foreach (var name in Enum.GetNames(typeof(T)))
            result.Add(name.ToLower());
        AllowedEnumValuesCache[typeof(T)] = result;
        return result;
    }

    public static string GetAllowedEnumValuesMessage<T>() where T : Enum
    {
        var allowed = GetAllowedEnumValues<T>();
        return $"Cannot convert {typeof(T).Name}, allowed values: {string.Join(", ", allowed)}.";
    }
}