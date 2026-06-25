namespace MADOC.Application.Documents;

public sealed class DocumentTypeDescriptor
{
    public DocumentTypeKey Key { get; }

    public string DisplayName { get; }

    public Type DocumentType { get; }

    public string TemplateFileName { get; }

    public DocumentTypeDescriptor(DocumentTypeKey key,string displayName,Type documentType,string templateFileName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Отображаемое имя типа документа не может быть пустым", nameof(displayName));
        }

        ArgumentNullException.ThrowIfNull(documentType);

        if (!documentType.IsClass)
        {
            throw new ArgumentException("Тип документа должен быть ссылочным типом", nameof(documentType));
        }

        if (string.IsNullOrWhiteSpace(templateFileName))
        {
            throw new ArgumentException("Имя HTML-шаблона не может быть пустым", nameof(templateFileName));
        }

        Key = key;
        DisplayName = displayName;
        DocumentType = documentType;
        TemplateFileName = templateFileName;
    }
}
