using MADOC.Application.Documents;

namespace MADOC.Application.Printing;

public sealed class PrintFormResponse
{
    public DocumentTypeKey DocumentType { get; }

    public bool IsSuccess { get; }

    public string HtmlContent { get; }

    public IReadOnlyList<string> Errors { get; }

    private PrintFormResponse(DocumentTypeKey documentType,bool isSuccess,string htmlContent,IReadOnlyList<string> errors)
    {
        DocumentType = documentType;
        IsSuccess = isSuccess;
        HtmlContent = htmlContent;
        Errors = errors;
    }

    public static PrintFormResponse Success(DocumentTypeKey documentType, string htmlContent)
    {
        ArgumentNullException.ThrowIfNull(htmlContent);

        return new PrintFormResponse(documentType, true, htmlContent, Array.Empty<string>());
    }

    public static PrintFormResponse Failed(DocumentTypeKey documentType,string htmlContent,IEnumerable<string> errors)
    {
        ArgumentNullException.ThrowIfNull(htmlContent);
        ArgumentNullException.ThrowIfNull(errors);

        var errorList = errors
            .Where(error => !string.IsNullOrWhiteSpace(error))
            .ToArray();

        if (errorList.Length == 0)
        {
            throw new ArgumentException("Список ошибок печатной формы не может быть пустым", nameof(errors));
        }

        return new PrintFormResponse(documentType, false, htmlContent, errorList);
    }
}
