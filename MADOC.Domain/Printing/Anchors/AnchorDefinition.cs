namespace MADOC.Domain.Printing.Anchors;

public sealed class AnchorDefinition<TDocument> where TDocument : class
{
    private readonly Func<TDocument, object?> getValueFromDocument;

    public AnchorKey Key { get; }

    public string FieldName { get; }

    public AnchorDefinition(AnchorKey key, string fieldName, Func<TDocument, object?> getValueFromDocument)
    {
        ValidateFieldName(fieldName);
        ArgumentNullException.ThrowIfNull(getValueFromDocument);

        Key = key;
        FieldName = fieldName;
        this.getValueFromDocument = getValueFromDocument;
    }

    public object? GetValue(TDocument document)
    {
        ValidateDocument(document);

        return getValueFromDocument(document);
    }

    private static void ValidateFieldName(string fieldName)
    {
        if (string.IsNullOrWhiteSpace(fieldName))
        {
            throw new ArgumentException("Имя поля якоря не может быть пустым", nameof(fieldName));
        }
    }

    private static void ValidateDocument(TDocument document)
    {
        ArgumentNullException.ThrowIfNull(document);
    }
}