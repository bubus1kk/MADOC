using MADOC.Application.Forms;

namespace MADOC.Application.Lists;

public sealed class DocumentFieldOptionsResponse
{
    public bool IsSuccess { get; }

    public IReadOnlyList<DocumentFieldOptionDto> Options { get; }

    public IReadOnlyList<string> Errors { get; }

    private DocumentFieldOptionsResponse(
        bool isSuccess,
        IReadOnlyList<DocumentFieldOptionDto> options,
        IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Options = options;
        Errors = errors;
    }

    public static DocumentFieldOptionsResponse Success(IEnumerable<DocumentFieldOptionDto> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new DocumentFieldOptionsResponse(true, options.ToArray(), Array.Empty<string>());
    }

    public static DocumentFieldOptionsResponse Failed(IEnumerable<string> errors)
    {
        ArgumentNullException.ThrowIfNull(errors);

        var errorList = errors
            .Where(error => !string.IsNullOrWhiteSpace(error))
            .ToArray();

        if (errorList.Length == 0)
        {
            throw new ArgumentException("Список ошибок не может быть пустым.", nameof(errors));
        }

        return new DocumentFieldOptionsResponse(false, Array.Empty<DocumentFieldOptionDto>(), errorList);
    }
}
