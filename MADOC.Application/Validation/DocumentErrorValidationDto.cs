namespace MADOC.Application.Validation;

public sealed class DocumentValidationErrorDto
{
    public string FieldName { get; }

    public string Message { get; }

    public DocumentValidationErrorDto(string fieldName, string message)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Имя поля ошибки валидации не может быть пустым", nameof(fieldName));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Сообщение ошибки валидации не может быть пустым", nameof(message));
        }

        FieldName = fieldName;
        Message = message;
    }
}
