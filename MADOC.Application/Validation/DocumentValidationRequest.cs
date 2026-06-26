using MADOC.Application.Documents;

namespace MADOC.Application.Validation;

public sealed class DocumentValidationRequest
{
    public DocumentTypeKey DocumentType { get; }

    public IReadOnlyDictionary<string, object?> Values { get; }

    public DocumentValidationRequest(DocumentTypeKey documentType, IReadOnlyDictionary<string, object?> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        DocumentType = documentType;
        Values = values;
    }
}
