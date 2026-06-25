using MADOC.Application.Documents;

namespace MADOC.Application.Forms;

public sealed class DocumentFormSchema
{
    public DocumentTypeKey DocumentType { get; }

    public string DisplayName { get; }

    public IReadOnlyList<DocumentFieldSchema> Fields { get; }

    public DocumentFormSchema(DocumentTypeKey documentType,string displayName,IEnumerable<DocumentFieldSchema> fields)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Отображаемое имя формы не может быть пустым", nameof(displayName));
        }

        ArgumentNullException.ThrowIfNull(fields);

        var fieldList = fields.ToArray();

        if (fieldList.Length == 0)
        {
            throw new ArgumentException("Схема формы должна содержать хотя бы одно поле", nameof(fields));
        }

        DocumentType = documentType;
        DisplayName = displayName;
        Fields = fieldList;
    }
}
