using System.Reflection;
using System.Text;

namespace MADOC.Domain.Printing.Anchors;

public static class AnchorNameGenerator
{
    private const string DocumentSuffix = "Document";

    public static AnchorKey Create(string prefix, PropertyInfo propertyInfo)
    {
        ArgumentNullException.ThrowIfNull(propertyInfo);

        if (string.IsNullOrWhiteSpace(prefix))
        {
            throw new ArgumentException(
                "Префикс якоря не может быть пустым.",
                nameof(prefix));
        }

        if (prefix.EndsWith(".", StringComparison.Ordinal))
        {
            throw new ArgumentException(
                "Префикс якоря не должен заканчиваться точкой.",
                nameof(prefix));
        }

        var propertyName = ToSnakeCase(propertyInfo.Name);

        return new AnchorKey($"{prefix}.{propertyName}");
    }

    public static string CreateDocumentPrefix(Type documentType)
    {
        ArgumentNullException.ThrowIfNull(documentType);

        var documentName = documentType.Name;

        if (documentName.EndsWith(DocumentSuffix, StringComparison.Ordinal))
        {
            documentName = documentName[..^DocumentSuffix.Length];
        }

        if (string.IsNullOrWhiteSpace(documentName))
        {
            throw new InvalidOperationException(
                $"Не удалось сформировать префикс якоря для типа {documentType.Name}.");
        }

        return ToSnakeCase(documentName);
    }

    public static string ToSnakeCase(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException(
                "Значение для преобразования в snake_case не может быть пустым.",
                nameof(value));
        }

        var builder = new StringBuilder();

        for (var i = 0; i < value.Length; i++)
        {
            var character = value[i];

            if (char.IsWhiteSpace(character))
            {
                continue;
            }

            if (char.IsUpper(character))
            {
                if (ShouldAddSeparator(value, i))
                {
                    builder.Append('_');
                }

                builder.Append(char.ToLowerInvariant(character));
                continue;
            }

            builder.Append(char.ToLowerInvariant(character));
        }

        return builder.ToString();
    }

    private static bool ShouldAddSeparator(string value, int index)
    {
        if (index == 0)
        {
            return false;
        }

        var previous = value[index - 1];

        if (previous == '_')
        {
            return false;
        }

        if (char.IsLower(previous) || char.IsDigit(previous))
        {
            return true;
        }

        if (char.IsUpper(previous) &&
            index + 1 < value.Length &&
            char.IsLower(value[index + 1]))
        {
            return true;
        }

        return false;
    }
}