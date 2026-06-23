using System.Net;
using System.Text;
using MADOC.Domain.Printing.Anchors;

namespace MADOC.Domain.Printing.Forms;

public sealed class HtmlPrintFormGenerator<TDocument>
    where TDocument : class
{
    private readonly PrintAnchorValueResolver<TDocument> documentValueResolver;

    private readonly bool strictMode;

    private readonly bool htmlEncodeValues;

    public HtmlPrintFormGenerator(bool strictMode = true, bool htmlEncodeValues = true) : this(new PrintAnchorProfile<TDocument>(), strictMode, htmlEncodeValues)
    {

    }

    public HtmlPrintFormGenerator(PrintAnchorProfile<TDocument> anchorProfile, bool strictMode = true, bool htmlEncodeValues = true)
    {
        ArgumentNullException.ThrowIfNull(anchorProfile);

        documentValueResolver = new PrintAnchorValueResolver<TDocument>(anchorProfile);

        this.strictMode = strictMode;
        this.htmlEncodeValues = htmlEncodeValues;
    }

    public HtmlPrintFormGenerationResult Generate(TDocument document, string htmlTemplate, IReadOnlyDictionary<string, string>? generatedFields = null)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(htmlTemplate);

        var errors = new List<string>();

        var safeGeneratedFields = generatedFields ?? new Dictionary<string, string>(StringComparer.Ordinal);

        var renderedHtml = ReplaceAnchors(document, htmlTemplate, safeGeneratedFields, errors);

        if (errors.Count > 0)
        {
            return HtmlPrintFormGenerationResult.Failed(renderedHtml, errors);
        }

        return HtmlPrintFormGenerationResult.Success(renderedHtml);
    }

    private string ReplaceAnchors(TDocument document, string htmlTemplate, IReadOnlyDictionary<string, string> generatedFields, List<string> errors)
    {
        var result = new StringBuilder();

        var currentPosition = 0;

        while (currentPosition < htmlTemplate.Length)
        {
            var openIndex = htmlTemplate.IndexOf("{{", currentPosition, StringComparison.Ordinal);

            if (openIndex < 0)
            {
                result.Append(htmlTemplate, currentPosition, htmlTemplate.Length - currentPosition);

                break;
            }

            var closeIndex = htmlTemplate.IndexOf("}}", openIndex + 2, StringComparison.Ordinal);

            if (closeIndex < 0)
            {
                result.Append(htmlTemplate, currentPosition, htmlTemplate.Length - currentPosition);

                break;
            }

            result.Append(htmlTemplate, currentPosition, openIndex - currentPosition);

            var originalAnchorText = htmlTemplate.Substring(openIndex, closeIndex - openIndex + 2);

            var replacement = ReplaceSingleAnchor(document, originalAnchorText, generatedFields, errors);

            result.Append(replacement);

            currentPosition = closeIndex + 2;
        }

        return result.ToString();
    }

    private string ReplaceSingleAnchor(TDocument document, string originalAnchorText, IReadOnlyDictionary<string, string> generatedFields, List<string> errors)
    {
        var anchor = CreateAnchorOrNull(originalAnchorText);

        if (anchor is null)
        {
            return HandleAnchorError(originalAnchorText, "Якорь имеет некорректный формат", errors);
        }

        try
        {
            var value = string.Empty;

            if (anchor.IsDocumentAnchor)
            {
                value = ResolveDocumentAnchor(document, anchor);
            }
            else
            {
                value = ResolveGeneratedAnchor(anchor, generatedFields);
            }

            return PrepareValueForHtml(value);
        }
        catch (Exception exception)
        {
            return HandleAnchorError(originalAnchorText, exception.Message, errors);
        }
    }

    private string ResolveDocumentAnchor(TDocument document, HtmlTemplateAnchor anchor)
    {
        return documentValueResolver.GetDisplayValue(document, anchor.Name);
    }

    private static string ResolveGeneratedAnchor(HtmlTemplateAnchor anchor, IReadOnlyDictionary<string, string> generatedFields)
    {
        if (generatedFields.TryGetValue(anchor.Name, out var value))
        {
            return value;
        }

        throw new InvalidOperationException($"Сгенерированное поле {anchor.OriginalText} не найдено");
    }

    private string HandleAnchorError(string originalAnchorText, string errorMessage, List<string> errors)
    {
        if (strictMode)
        {
            errors.Add($"Не удалось обработать якорь {originalAnchorText}: {errorMessage}");

            return originalAnchorText;
        }

        return string.Empty;
    }

    private string PrepareValueForHtml(string value)
    {
        if (!htmlEncodeValues)
        {
            return value;
        }

        return WebUtility.HtmlEncode(value);
    }

    private static HtmlTemplateAnchor? CreateAnchorOrNull(string originalAnchorText)
    {
        if (originalAnchorText.Length < 4)
        {
            return null;
        }

        var content = originalAnchorText.Substring(2, originalAnchorText.Length - 4).Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            return null;
        }

        var isDocumentAnchor = TryGetDocumentAnchorName(content, out var anchorName);

        if (!isDocumentAnchor)
        {
            anchorName = content;
        }

        if (string.IsNullOrWhiteSpace(anchorName))
        {
            return null;
        }

        if (!ContainsOnlyAllowedCharacters(anchorName))
        {
            return null;
        }

        return new HtmlTemplateAnchor(originalAnchorText, anchorName, isDocumentAnchor);
    }

    private static bool TryGetDocumentAnchorName(string content, out string anchorName)
    {
        anchorName = string.Empty;

        if (!content.StartsWith("doc", StringComparison.Ordinal))
        {
            return false;
        }

        var currentIndex = 3;

        while (currentIndex < content.Length && char.IsWhiteSpace(content[currentIndex]))
        {
            currentIndex++;
        }

        if (currentIndex >= content.Length || content[currentIndex] != ':')
        {
            return false;
        }

        anchorName = content[(currentIndex + 1)..].Trim();

        return true;
    }

    private static bool ContainsOnlyAllowedCharacters(string value)
    {
        foreach (var character in value)
        {
            if (IsLatinLetter(character) || char.IsDigit(character))
            {
                continue;
            }

            if (character == '.' || character == '_' || character == '-')
            {
                continue;
            }

            return false;
        }

        return true;
    }

    private static bool IsLatinLetter(char character)
    {
        return character is >= 'a' and <= 'z' or >= 'A' and <= 'Z';
    }

    private sealed class HtmlTemplateAnchor
    {
        public string OriginalText { get; }

        public string Name { get; }

        public bool IsDocumentAnchor { get; }

        public HtmlTemplateAnchor(string originalText, string name, bool isDocumentAnchor)
        {
            OriginalText = originalText;
            Name = name;
            IsDocumentAnchor = isDocumentAnchor;
        }
    }
}
