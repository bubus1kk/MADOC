namespace MADOC.Application.Documents;

public sealed class DocumentValues
{
    private readonly Dictionary<string, object?> values;

    public IReadOnlyDictionary<string, object?> Values
    {
        get
        {
            return values;
        }
    }

    public DocumentValues(IReadOnlyDictionary<string, object?> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        this.values = new Dictionary<string, object?>(values, StringComparer.Ordinal);
    }

    public static DocumentValues Empty { get; } = new(new Dictionary<string, object?>());
}
