using MADOC.Printing.Html.Anchors;
using MADOC.Printing.Html.Forms;

namespace MADOC.Application.Printing;

public sealed class RuntimeHtmlPrintFormGenerator<TDocument>
    where TDocument : class
{
    private readonly HtmlPrintFormGenerator<TDocument> generator;

    private readonly IReadOnlyDictionary<string, string> generatedFields;

    public RuntimeHtmlPrintFormGenerator(
        PrintAnchorProfile<TDocument> anchorProfile,
        IReadOnlyDictionary<string, string>? generatedFields = null,
        bool strictMode = true,
        bool htmlEncodeValues = true)
    {
        ArgumentNullException.ThrowIfNull(anchorProfile);

        generator = new HtmlPrintFormGenerator<TDocument>(
            anchorProfile,
            strictMode,
            htmlEncodeValues);

        this.generatedFields = generatedFields ?? new Dictionary<string, string>(StringComparer.Ordinal);
    }

    public HtmlPrintFormGenerationResult Generate(
        TDocument document,
        string htmlTemplate)
    {
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(htmlTemplate);

        return generator.Generate(
            document,
            htmlTemplate,
            generatedFields);
    }
}