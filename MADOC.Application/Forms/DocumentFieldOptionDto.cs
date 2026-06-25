namespace MADOC.Application.Forms;

public sealed class DocumentFieldOptionDto
{
    public string Value { get; }

    public string DisplayName { get; }

    public DocumentFieldOptionDto(string value, string displayName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Значение варианта поля не может быть пустым.", nameof(value));
        }

        if (string.IsNullOrWhiteSpace(displayName))
        {
            throw new ArgumentException("Отображаемое имя варианта поля не может быть пустым.", nameof(displayName));
        }

        Value = value;
        DisplayName = displayName;
    }
}
