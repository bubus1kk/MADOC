namespace MADOC.Application.Documents;

public sealed class DocumentMappingError
{
    public string FieldName { get; }

    public string Message { get; }

    public DocumentMappingError(string fieldName, string message)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Имя поля ошибки маппинга не может быть пустым.", nameof(fieldName));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Сообщение ошибки маппинга не может быть пустым.", nameof(message));
        }

        FieldName = fieldName;
        Message = message;
    }
}
