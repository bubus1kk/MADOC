using System.Text.Json;

namespace MADOC.DesktopBridge.Commands;

public sealed class GetFieldOptionsPayload
{
    public string DocumentType { get; init; } = string.Empty;

    public string FieldName { get; init; } = string.Empty;

    public Dictionary<string, JsonElement>? Values { get; init; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(DocumentType))
        {
            throw new ArgumentException("Поле documentType обязательно");
        }

        if (string.IsNullOrWhiteSpace(FieldName))
        {
            throw new ArgumentException("Поле fieldName обязательно");
        }
    }
}
