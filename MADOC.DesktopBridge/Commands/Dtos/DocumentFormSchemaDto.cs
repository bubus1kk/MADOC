using MADOC.Application.Forms;

namespace MADOC.DesktopBridge.Commands;

public sealed class DocumentFormSchemaDto
{
    public string DocumentType { get; }

    public string DisplayName { get; }

    public IReadOnlyList<DocumentFieldSchemaDto> Fields { get; }

    public DocumentFormSchemaDto(string documentType, string displayName, IEnumerable<DocumentFieldSchemaDto> fields)
    {
        DocumentType = documentType;
        DisplayName = displayName;
        Fields = fields.ToArray();
    }

    public static DocumentFormSchemaDto FromApplicationSchema(DocumentFormSchema schema)
    {
        ArgumentNullException.ThrowIfNull(schema);

        return new DocumentFormSchemaDto(
            schema.DocumentType.Value,
            schema.DisplayName,
            schema.Fields.Select(DocumentFieldSchemaDto.FromApplicationSchema));
    }
}
