using System.Globalization;
using System.Text.Json;

namespace MADOC.Application.Printing;

internal static class GeneratedFieldValueNormalizer
{
    public static IReadOnlyDictionary<string, string> Normalize(IReadOnlyDictionary<string, object?> generatedFields)
    {
        ArgumentNullException.ThrowIfNull(generatedFields);

        var result = new Dictionary<string, string>(StringComparer.Ordinal);

        foreach (var field in generatedFields)
        {
            if (string.IsNullOrWhiteSpace(field.Key))
            {
                throw new ArgumentException("Ключ сгенерированного поля не может быть пустым.", nameof(generatedFields));
            }

            result.Add(field.Key, ConvertToString(field.Value));
        }

        return result;
    }

    private static string ConvertToString(object? value)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (value is JsonElement jsonElement)
        {
            return jsonElement.ValueKind == JsonValueKind.String
                ? jsonElement.GetString() ?? string.Empty
                : jsonElement.ToString();
        }

        return value switch
        {
            DateOnly date => date.ToString("dd.MM.yyyy", CultureInfo.InvariantCulture),
            TimeOnly time => time.ToString("HH:mm", CultureInfo.InvariantCulture),
            DateTime dateTime => dateTime.ToString("dd.MM.yyyy HH:mm", CultureInfo.InvariantCulture),
            bool boolean => boolean ? "Да" : "Нет",
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
            _ => value.ToString() ?? string.Empty
        };
    }
}
