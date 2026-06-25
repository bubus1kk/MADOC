namespace MADOC.Application.Documents;

public readonly record struct DocumentTypeKey
{
    public string Value { get; }

    public DocumentTypeKey(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Ключ типа документа не может быть пустым", nameof(value));
        }

        Value = value.Trim();
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator DocumentTypeKey(string value)
    {
        return new DocumentTypeKey(value);
    }
}
