using MADOC.Domain.Core.Ranges;
using MADOC.Domain.Core.Validation.Lists;
using System.Globalization;
using System.Reflection;
using System.Text.Json;


namespace MADOC.Application.Documents;

public sealed class DocumentValueMapper
{
    private static readonly string[] RangeFromKeys = { "from", "From", "start", "Start", "begin", "Begin" };

    private static readonly string[] RangeToKeys = { "to", "To", "end", "End" };

    public DocumentMappingResult MapValues(object document, IReadOnlyDictionary<string, object?> values, bool rejectUnknownFields = true)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(values);

        var errors = new List<DocumentMappingError>();
        var documentType = document.GetType();
        var writableProperties = GetWritableProperties(documentType);

        if (rejectUnknownFields)
        {
            AddUnknownFieldErrors(values, writableProperties, errors);
        }

        var convertedValues = new List<(PropertyInfo Property, object? Value)>();

        foreach (var valuePair in values)
        {
            if (!writableProperties.TryGetValue(valuePair.Key, out var property))
            {
                continue;
            }

            try
            {
                var convertedValue = ConvertValue(valuePair.Value, property.PropertyType);
                convertedValues.Add((property, convertedValue));
            }
            catch (Exception exception)
            {
                errors.Add(new DocumentMappingError(property.Name, $"Не удалось преобразовать значение поля {property.Name}: {exception.Message}"));
            }
        }

        if (errors.Count > 0)
        {
            return DocumentMappingResult.Failed(errors);
        }

        foreach (var convertedValue in convertedValues)
        {
            convertedValue.Property.SetValue(document, convertedValue.Value);
        }

        return DocumentMappingResult.Success();
    }

    private static Dictionary<string, PropertyInfo> GetWritableProperties(Type documentType)
    {
        return documentType
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetIndexParameters().Length == 0)
            .Where(property => property.SetMethod is not null && property.SetMethod.IsPublic)
            .ToDictionary(property => property.Name, StringComparer.Ordinal);
    }

    private static void AddUnknownFieldErrors(IReadOnlyDictionary<string, object?> values, IReadOnlyDictionary<string, PropertyInfo> writableProperties,
        List<DocumentMappingError> errors)
    {
        foreach (var valuePair in values)
        {
            if (!writableProperties.ContainsKey(valuePair.Key))
            {
                errors.Add(new DocumentMappingError(valuePair.Key, $"Поле {valuePair.Key} не найдено среди изменяемых полей документа"));
            }
        }
    }

    private static object? ConvertValue(object? value, Type targetType)
    {
        var nonNullableType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (IsNullLikeValue(value))
        {
            if (targetType == typeof(string))
            {
                return string.Empty;
            }

            if (Nullable.GetUnderlyingType(targetType) is not null || !targetType.IsValueType)
            {
                return null;
            }

            throw new InvalidOperationException($"Поле типа {targetType.Name} не может получить пустое значение");
        }

        if (nonNullableType == typeof(string))
        {
            return ConvertToString(value);
        }

        if (nonNullableType == typeof(int))
        {
            return ConvertToInt32(value);
        }

        if (nonNullableType == typeof(long))
        {
            return ConvertToInt64(value);
        }

        if (nonNullableType == typeof(double))
        {
            return ConvertToDouble(value);
        }

        if (nonNullableType == typeof(decimal))
        {
            return ConvertToDecimal(value);
        }

        if (nonNullableType == typeof(float))
        {
            return Convert.ToSingle(ConvertToDouble(value), CultureInfo.InvariantCulture);
        }

        if (nonNullableType == typeof(bool))
        {
            return ConvertToBoolean(value);
        }

        if (nonNullableType == typeof(DateOnly))
        {
            return ConvertToDateOnly(value);
        }

        if (nonNullableType == typeof(TimeOnly))
        {
            return ConvertToTimeOnly(value);
        }

        if (nonNullableType == typeof(DateTime))
        {
            return ConvertToDateTime(value);
        }

        if (nonNullableType == typeof(ListOptionKey))
        {
            return ConvertToListOptionKey(value);
        }

        if (nonNullableType == typeof(DateRange))
        {
            return ConvertToDateRange(value);
        }

        if (nonNullableType == typeof(DateTimeRange))
        {
            return ConvertToDateTimeRange(value);
        }

        if (nonNullableType.IsEnum)
        {
            return ConvertToEnum(value, nonNullableType);
        }

        throw new NotSupportedException($"Тип поля {targetType.Name} не поддерживается маппером документа");
    }

    private static bool IsNullLikeValue(object? value)
    {
        if (value is null)
        {
            return true;
        }

        if (value is string text && string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined ||
                   jsonElement.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(jsonElement.GetString());
        }

        return false;
    }

    private static string ConvertToString(object value)
    {
        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.String
                ? jsonElement.GetString() ?? string.Empty
                : jsonElement.ToString();
        }

        return value.ToString() ?? string.Empty;
    }

    private static int ConvertToInt32(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Number && jsonElement.TryGetInt32(out var jsonNumber))
            {
                return jsonNumber;
            }

            return ParseInt32(jsonElement.ToString());
        }

        if (value is int number)
        {
            return number;
        }

        if (value is IConvertible convertible)
        {
            return Convert.ToInt32(convertible, CultureInfo.InvariantCulture);
        }

        return ParseInt32(value.ToString());
    }

    private static int ParseInt32(string? value)
    {
        if (int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new FormatException($"{value} не является целым числом");
    }

    private static long ConvertToInt64(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Number && jsonElement.TryGetInt64(out var jsonNumber))
            {
                return jsonNumber;
            }

            return ParseInt64(jsonElement.ToString());
        }

        if (value is long number)
        {
            return number;
        }

        if (value is IConvertible convertible)
        {
            return Convert.ToInt64(convertible, CultureInfo.InvariantCulture);
        }

        return ParseInt64(value.ToString());
    }

    private static long ParseInt64(string? value)
    {
        if (long.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new FormatException($"{value} не является целым числом");
    }

    private static double ConvertToDouble(object value)
    {
        if (value is string text)
        {
            return ParseDouble(text);
        }

        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Number && jsonElement.TryGetDouble(out var jsonNumber))
            {
                return jsonNumber;
            }

            return ParseDouble(jsonElement.ToString());
        }

        if (value is double number)
        {
            return number;
        }

        if (value is IConvertible convertible)
        {
            return Convert.ToDouble(convertible, CultureInfo.InvariantCulture);
        }

        return ParseDouble(value.ToString());
    }

    private static double ParseDouble(string? value)
    {
        var normalizedValue = value?.Replace(',', '.');

        if (double.TryParse(normalizedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new FormatException($"{value} не является числом");
    }

    private static decimal ConvertToDecimal(object value)
    {
        if (value is string text)
        {
            return ParseDecimal(text);
        }

        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Number && jsonElement.TryGetDecimal(out var jsonNumber))
            {
                return jsonNumber;
            }

            return ParseDecimal(jsonElement.ToString());
        }

        if (value is decimal number)
        {
            return number;
        }

        if (value is IConvertible convertible)
        {
            return Convert.ToDecimal(convertible, CultureInfo.InvariantCulture);
        }

        return ParseDecimal(value.ToString());
    }

    private static decimal ParseDecimal(string? value)
    {
        var normalizedValue = value?.Replace(',', '.');

        if (decimal.TryParse(normalizedValue, NumberStyles.Float, CultureInfo.InvariantCulture, out var result))
        {
            return result;
        }

        throw new FormatException($"{value} не является числом");
    }

    private static bool ConvertToBoolean(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind is JsonValueKind.True or JsonValueKind.False)
            {
                return jsonElement.GetBoolean();
            }

            return ParseBoolean(jsonElement.ToString());
        }

        if (value is bool boolean)
        {
            return boolean;
        }

        return ParseBoolean(value.ToString());
    }

    private static bool ParseBoolean(string? value)
    {
        var normalizedValue = value?.Trim().ToLowerInvariant();

        return normalizedValue switch
        {
            "true" or "1" or "yes" or "y" or "да" => true,
            "false" or "0" or "no" or "n" or "нет" => false,
            _ => throw new FormatException($"{value} не является логическим значением")
        };
    }

    private static DateOnly ConvertToDateOnly(object value)
    {
        if (value is JsonElement jsonElement)
        {
            return ConvertToDateOnly(jsonElement.ToString());
        }

        if (value is DateOnly dateOnly)
        {
            return dateOnly;
        }

        if (value is DateTime dateTime)
        {
            return DateOnly.FromDateTime(dateTime);
        }

        return ParseDateOnly(value.ToString());
    }

    private static DateOnly ParseDateOnly(string? value)
    {
        var formats = new[] { "yyyy-MM-dd", "dd.MM.yyyy", "dd-MM-yyyy", "yyyy.MM.dd" };

        if (DateOnly.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        if (DateOnly.TryParse(value, CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out result))
        {
            return result;
        }

        throw new FormatException($"{value} не является датой");
    }

    private static TimeOnly ConvertToTimeOnly(object value)
    {
        if (value is JsonElement jsonElement)
        {
            return ConvertToTimeOnly(jsonElement.ToString());
        }

        if (value is TimeOnly timeOnly)
        {
            return timeOnly;
        }

        if (value is DateTime dateTime)
        {
            return TimeOnly.FromDateTime(dateTime);
        }

        return ParseTimeOnly(value.ToString());
    }

    private static TimeOnly ParseTimeOnly(string? value)
    {
        var formats = new[] { "HH:mm", "H:mm", "HH:mm:ss" };

        if (TimeOnly.TryParseExact(value, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        throw new FormatException($"{value} не является временем");
    }

    private static DateTime ConvertToDateTime(object value)
    {
        if (value is JsonElement jsonElement)
        {
            return ConvertToDateTime(jsonElement.ToString());
        }

        if (value is DateTime dateTime)
        {
            return dateTime;
        }

        var formats = new[]
        {
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy-MM-dd HH:mm",
            "dd.MM.yyyy HH:mm:ss",
            "dd.MM.yyyy HH:mm"
        };

        if (DateTime.TryParseExact(value.ToString(), formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            return result;
        }

        if (DateTime.TryParse(value.ToString(), CultureInfo.GetCultureInfo("ru-RU"), DateTimeStyles.None, out result))
        {
            return result;
        }

        throw new FormatException($"{value} не является датой и временем");
    }

    private static ListOptionKey ConvertToListOptionKey(object value)
    {
        if (value is ListOptionKey key)
        {
            return key;
        }

        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.String)
            {
                return new ListOptionKey(jsonElement.GetString() ?? string.Empty);
            }

            if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                if (TryGetJsonProperty(jsonElement, "value", out var valueProperty))
                {
                    return new ListOptionKey(valueProperty.ToString());
                }

                if (TryGetJsonProperty(jsonElement, "key", out var keyProperty))
                {
                    if (keyProperty.ValueKind == JsonValueKind.Object && TryGetJsonProperty(keyProperty, "value", out var keyValueProperty))
                    {
                        return new ListOptionKey(keyValueProperty.ToString());
                    }

                    return new ListOptionKey(keyProperty.ToString());
                }

                throw new FormatException("JSON-объект варианта списка должен содержать поле value или key");
            }

            return new ListOptionKey(jsonElement.ToString());
        }

        return new ListOptionKey(value.ToString() ?? string.Empty);
    }

    private static DateRange ConvertToDateRange(object value)
    {
        var rangeValues = ExtractRangeValues(value);

        return new DateRange(ConvertToDateOnly(rangeValues.From), ConvertToDateOnly(rangeValues.To));
    }

    private static DateTimeRange ConvertToDateTimeRange(object value)
    {
        var rangeValues = ExtractRangeValues(value);

        return new DateTimeRange(ConvertToDateTime(rangeValues.From), ConvertToDateTime(rangeValues.To));
    }

    private static (object From, object To) ExtractRangeValues(object value)
    {
        if (value is JsonElement jsonElement)
        {
            if (jsonElement.ValueKind == JsonValueKind.Object)
            {
                var from = GetRequiredJsonRangeValue(jsonElement, RangeFromKeys);
                var to = GetRequiredJsonRangeValue(jsonElement, RangeToKeys);

                return (from, to);
            }

            return ParseRangeString(jsonElement.ToString());
        }

        if (value is IReadOnlyDictionary<string, object?> readOnlyDictionary)
        {
            return (GetRequiredDictionaryRangeValue(readOnlyDictionary, RangeFromKeys),
                GetRequiredDictionaryRangeValue(readOnlyDictionary, RangeToKeys));
        }

        if (value is IDictionary<string, object?> dictionary)
        {
            return (GetRequiredDictionaryRangeValue(dictionary, RangeFromKeys),
                GetRequiredDictionaryRangeValue(dictionary, RangeToKeys));
        }

        return ParseRangeString(value.ToString());
    }

    private static (string From, string To) ParseRangeString(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new FormatException("Диапазон не может быть пустым");
        }

        var separators = new[] { "..", "-", " - ", ";" };

        foreach (var separator in separators)
        {
            var parts = value.Split(separator, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 2)
            {
                return (parts[0], parts[1]);
            }
        }

        throw new FormatException($"Диапазон {value} должен содержать начало и конец");
    }

    private static object GetRequiredJsonRangeValue(JsonElement jsonElement, IReadOnlyList<string> keys)
    {
        foreach (var key in keys)
        {
            if (TryGetJsonProperty(jsonElement, key, out var property))
            {
                return property;
            }
        }

        throw new FormatException($"В диапазоне не найдено поле {string.Join("/", keys)}");
    }

    private static bool TryGetJsonProperty(JsonElement jsonElement, string propertyName, out JsonElement property)
    {
        foreach (var jsonProperty in jsonElement.EnumerateObject())
        {
            if (string.Equals(jsonProperty.Name, propertyName, StringComparison.OrdinalIgnoreCase))
            {
                property = jsonProperty.Value;
                return true;
            }
        }

        property = default;
        return false;
    }

    private static object GetRequiredDictionaryRangeValue(IReadOnlyDictionary<string, object?> dictionary, IReadOnlyList<string> keys)
    {
        foreach (var key in keys)
        {
            if (dictionary.TryGetValue(key, out var value) && value is not null)
            {
                return value;
            }

            var matchingKey = dictionary.Keys.FirstOrDefault(existingKey =>
                string.Equals(existingKey, key, StringComparison.OrdinalIgnoreCase));

            if (matchingKey is not null && dictionary[matchingKey] is not null)
            {
                return dictionary[matchingKey]!;
            }
        }

        throw new FormatException($"В диапазоне не найдено поле {string.Join("/", keys)}");
    }

    private static object GetRequiredDictionaryRangeValue(IDictionary<string, object?> dictionary, IReadOnlyList<string> keys)
    {
        foreach (var key in keys)
        {
            if (dictionary.TryGetValue(key, out var value) && value is not null)
            {
                return value;
            }

            var matchingKey = dictionary.Keys.FirstOrDefault(existingKey =>
                string.Equals(existingKey, key, StringComparison.OrdinalIgnoreCase));

            if (matchingKey is not null && dictionary[matchingKey] is not null)
            {
                return dictionary[matchingKey]!;
            }
        }

        throw new FormatException($"В диапазоне не найдено поле {string.Join("/", keys)}");
    }

    private static object ConvertToEnum(object value, Type enumType)
    {
        if (value is JsonElement jsonElement)
        {
            value = jsonElement.ToString();
        }

        if (value is string text)
        {
            return Enum.Parse(enumType, text, ignoreCase: true);
        }

        return Enum.ToObject(enumType, value);
    }
}
