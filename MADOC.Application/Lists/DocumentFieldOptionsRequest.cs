using MADOC.Application.Documents;

namespace MADOC.Application.Lists;

public sealed class DocumentFieldOptionsRequest
{
    public DocumentTypeKey DocumentType { get; }

    public string FieldName { get; }

    public IReadOnlyDictionary<string, object?> Values { get; }

    public DocumentFieldOptionsRequest(
        DocumentTypeKey documentType,
        string fieldName,
        IReadOnlyDictionary<string, object?>? values = null)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Имя поля списка не может быть пустым.", nameof(fieldName));
        }

        DocumentType = documentType;
        FieldName = fieldName;
        Values = values ?? new Dictionary<string, object?>();
    }
}
