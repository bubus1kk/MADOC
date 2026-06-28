using MADOC.Application.Forms;

namespace MADOC.DesktopBridge.Commands;

public sealed class DocumentFieldSchemaDto
{
    public string Name { get; }

    public string DisplayName { get; }

    public string Type { get; }

    public bool IsReadOnly { get; }

    public string AnchorName { get; }

    public DocumentFieldConstraintDto Constraints { get; }

    public IReadOnlyList<DocumentFieldOptionDto> Options { get; }

    public DocumentFieldDependencyDto? Dependency { get; }

    public DocumentFieldSchemaDto(string name,string displayName,string type,bool isReadOnly,string anchorName,
        DocumentFieldConstraintDto constraints,IEnumerable<DocumentFieldOptionDto> options,DocumentFieldDependencyDto? dependency)
    {
        Name = name;
        DisplayName = displayName;
        Type = type;
        IsReadOnly = isReadOnly;
        AnchorName = anchorName;
        Constraints = constraints;
        Options = options.ToArray();
        Dependency = dependency;
    }

    public static DocumentFieldSchemaDto FromApplicationSchema(DocumentFieldSchema field)
    {
        ArgumentNullException.ThrowIfNull(field);

        return new DocumentFieldSchemaDto(
            field.Name,
            field.DisplayName,
            field.Type.ToString(),
            field.IsReadOnly,
            field.AnchorName,
            field.Constraints,
            field.Options,
            field.Dependency);
    }
}
