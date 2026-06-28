using System.Text.Json;

namespace MADOC.DesktopBridge.Serialization;

public static class BridgeValueDictionary
{
    public static IReadOnlyDictionary<string, object?> Empty { get; } = new Dictionary<string, object?>(StringComparer.Ordinal);

    public static IReadOnlyDictionary<string, object?> From(Dictionary<string, JsonElement>? values)
    {
        if (values is null || values.Count == 0)
        {
            return Empty;
        }

        return values.ToDictionary(
            pair => pair.Key,
            pair => ConvertJsonElement(pair.Value),
            StringComparer.Ordinal);
    }

    private static object? ConvertJsonElement(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.Null => null,
            JsonValueKind.Undefined => null,
            JsonValueKind.String => element.GetString(),
            JsonValueKind.Number => ConvertNumber(element),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Object => element.Clone(),
            JsonValueKind.Array => element.Clone(),
            _ => element.Clone()
        };
    }

    private static object ConvertNumber(JsonElement element)
    {
        if (element.TryGetInt32(out var intValue))
        {
            return intValue;
        }

        if (element.TryGetInt64(out var longValue))
        {
            return longValue;
        }

        if (element.TryGetDecimal(out var decimalValue))
        {
            return decimalValue;
        }

        return element.GetDouble();
    }
}
