namespace MADOC.Application.Forms;

public sealed class DocumentFieldSchema
{
    public string Name { get; }

    public string DisplayName { get; }

    public DocumentFieldType Type { get; }

    public bool IsReadOnly { get; }

    public string AnchorName { get; }

    public DocumentFieldConstraintDto Constraints { get; }

    public IReadOnlyList<DocumentFieldOptionDto> Options { get; }

    public DocumentFieldDependencyDto? Dependency { get; }

    public DocumentFieldSchema(string name,string displayName,DocumentFieldType type,bool isReadOnly,string anchorName,
        DocumentFieldConstraintDto constraints,IEnumerable<DocumentFieldOptionDto>? options = null,
        DocumentFieldDependencyDto? dependency = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Имя поля не может быть пустым", nameof(name));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Отображаемое имя поля не может быть пустым", nameof(displayName));
        }

        if (string.IsNullOrWhiteSpace(anchorName))
        {
            throw new ArgumentException("Имя печатного якоря поля не может быть пустым", nameof(anchorName));
        }

        ArgumentNullException.ThrowIfNull(constraints);

        Name = name;
        DisplayName = displayName;
        Type = type;
        IsReadOnly = isReadOnly;
        AnchorName = anchorName;
        Constraints = constraints;
        Options = options?.ToArray() ?? Array.Empty<DocumentFieldOptionDto>();
        Dependency = dependency;
    }
}
