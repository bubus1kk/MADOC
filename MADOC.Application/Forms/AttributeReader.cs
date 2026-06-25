using System.Reflection;

namespace MADOC.Application.Forms;

internal static class AttributeReader
{
    public static bool HasAttribute(PropertyInfo property, string attributeTypeName)
    {
        return property.GetCustomAttributes(inherit: true)
            .Any(attribute => attribute.GetType().Name == attributeTypeName);
    }

    public static Attribute? FindAttribute(PropertyInfo property, string attributeTypeName)
    {
        return property.GetCustomAttributes(inherit: true)
            .OfType<Attribute>()
            .FirstOrDefault(attribute => attribute.GetType().Name == attributeTypeName);
    }

    public static T? ReadProperty<T>(object? source, string propertyName)
    {
        if (source is null)
        {
            return default;
        }

        var property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        if (property is null)
        {
            return default;
        }

        var value = property.GetValue(source);

        if (value is null)
        {
            return default;
        }

        if (value is T typedValue)
        {
            return typedValue;
        }

        if (typeof(T) == typeof(string))
        {
            return (T)(object)value.ToString()!;
        }

        if (typeof(T).IsEnum && value is string text)
        {
            return (T)Enum.Parse(typeof(T), text, ignoreCase: true);
        }

        return (T)Convert.ChangeType(value, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));
    }

    public static string? ReadAsString(object? source, string propertyName)
    {
        if (source is null)
        {
            return null;
        }

        var property = source.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);

        return property?.GetValue(source)?.ToString();
    }
}
