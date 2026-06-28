namespace MADOC.DesktopBridge.Commands;

public sealed class DocumentTypeDto
{
    public string Key { get; }

    public string DisplayName { get; }

    public string TemplateFileName { get; }

    public DocumentTypeDto(string key, string displayName, string templateFileName)
    {
        Key = key;
        DisplayName = displayName;
        TemplateFileName = templateFileName;
    }
}
