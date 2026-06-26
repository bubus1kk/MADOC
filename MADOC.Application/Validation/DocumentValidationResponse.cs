using MADOC.Application.Documents;

namespace MADOC.Application.Validation;

public sealed class DocumentValidationResponse
{
    public DocumentTypeKey DocumentType { get; }

    public bool IsValid { get; }

    public IReadOnlyList<DocumentValidationErrorDto> Errors { get; }

    public DocumentValidationResponse(DocumentTypeKey documentType, bool isValid, IEnumerable<DocumentValidationErrorDto> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var errorList = errors.ToArray();

        if (isValid && errorList.Length > 0)
        {
            throw new ArgumentException("Валидный результат не может содержать ошибки", nameof(errors));
        }

        DocumentType = documentType;
        IsValid = isValid;
        Errors = errorList;
    }
}
