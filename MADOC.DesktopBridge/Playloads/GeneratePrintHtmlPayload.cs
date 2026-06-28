using System.Text.Json;

namespace MADOC.DesktopBridge.Commands;

public sealed class GeneratePrintHtmlPayload
{
    public string DocumentType { get; init; } = string.Empty;

    public Dictionary<string, JsonElement>? Values { get; init; }

    public Dictionary<string, JsonElement>? GeneratedFields { get; init; }

    public string? TemplateFileName { get; init; }

    public bool StrictMode { get; init; } = true;

    public bool HtmlEncodeValues { get; init; } = true;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(DocumentType))
        {
            throw new ArgumentException("Поле documentType обязательно");
        }
    }
}
