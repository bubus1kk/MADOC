using MADOC.Application.Documents;

namespace MADOC.Application.Printing;

public sealed class PrintFormRequest
{
    public DocumentTypeKey DocumentType { get; }

    public IReadOnlyDictionary<string, object?> Values { get; }

    public IReadOnlyDictionary<string, object?> GeneratedFields { get; }

    public string? TemplateFileName { get; }

    public bool StrictMode { get; }

    public bool HtmlEncodeValues { get; }

    public PrintFormRequest(DocumentTypeKey documentType,IReadOnlyDictionary<string, object?> values,
        IReadOnlyDictionary<string, object?>? generatedFields = null,string? templateFileName = null,
        bool strictMode = true,bool htmlEncodeValues = true)
    {
        ArgumentNullException.ThrowIfNull(values);

        DocumentType = documentType;
        Values = values;
        GeneratedFields = generatedFields ?? new Dictionary<string, object?>();
        TemplateFileName = templateFileName;
        StrictMode = strictMode;
        HtmlEncodeValues = htmlEncodeValues;
    }
}
