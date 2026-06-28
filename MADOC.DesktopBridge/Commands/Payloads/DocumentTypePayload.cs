namespace MADOC.DesktopBridge.Commands;

public sealed class DocumentTypePayload
{
    public string DocumentType { get; init; } = string.Empty;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(DocumentType))
        {
            throw new ArgumentException("Поле documentType обязательно");
        }
    }
}
