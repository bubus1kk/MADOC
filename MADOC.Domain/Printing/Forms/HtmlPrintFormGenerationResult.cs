namespace MADOC.Domain.Printing.Forms;

public sealed class HtmlPrintFormGenerationResult
{
    public string Content { get; }

    public IReadOnlyList<string> Errors { get; }

    public bool IsSuccess
    {
        get
        {
            return Errors.Count == 0;
        }
    }

    private HtmlPrintFormGenerationResult(string content, IReadOnlyList<string> errors)
    {
        Content = content;
        Errors = errors;
    }

    public static HtmlPrintFormGenerationResult Success(string content)
    {
        ArgumentNullException.ThrowIfNull(content);

        return new HtmlPrintFormGenerationResult(content, Array.Empty<string>());
    }

    public static HtmlPrintFormGenerationResult Failed(string content, IEnumerable<string> errors)
    {
        ArgumentNullException.ThrowIfNull(content);
        ArgumentNullException.ThrowIfNull(errors);

        var errorList = new List<string>();

        foreach (var error in errors)
        {
            if (!string.IsNullOrWhiteSpace(error))
            {
                errorList.Add(error);
            }
        }

        if (errorList.Count == 0)
        {
            throw new ArgumentException("Список ошибок не может быть пустым для неуспешного результата", nameof(errors));
        }

        return new HtmlPrintFormGenerationResult(content, errorList);
    }
}