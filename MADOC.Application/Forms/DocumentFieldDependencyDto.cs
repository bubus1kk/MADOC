namespace MADOC.Application.Forms;

public sealed class DocumentFieldDependencyDto
{
    public IReadOnlyList<string> ParentFieldNames { get; }

    public DocumentFieldDependencyDto(IEnumerable<string> parentFieldNames)
    {
        ArgumentNullException.ThrowIfNull(parentFieldNames);

        var fieldNames = parentFieldNames
            .Where(fieldName => !string.IsNullOrWhiteSpace(fieldName))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (fieldNames.Length == 0)
        {
            throw new ArgumentException("Список родительских полей зависимости не может быть пустым.", nameof(parentFieldNames));
        }

        ParentFieldNames = fieldNames;
    }
}
