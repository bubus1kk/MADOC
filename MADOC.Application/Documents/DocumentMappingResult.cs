namespace MADOC.Application.Documents;

public sealed class DocumentMappingResult
{
    public IReadOnlyList<DocumentMappingError> Errors { get; }

    public bool IsSuccess
    {
        get
        {
            return Errors.Count == 0;
        }
    }

    private DocumentMappingResult(IReadOnlyList<DocumentMappingError> errors)
    {
        Errors = errors;
    }

    public static DocumentMappingResult Success()
    {
        return new DocumentMappingResult(Array.Empty<DocumentMappingError>());
    }

    public static DocumentMappingResult Failed(IEnumerable<DocumentMappingError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var errorList = errors.ToArray();

        if (errorList.Length == 0)
        {
            throw new ArgumentException("Нельзя создать неуспешный результат маппинга без ошибок.", nameof(errors));
        }

        return new DocumentMappingResult(errorList);
    }
}
