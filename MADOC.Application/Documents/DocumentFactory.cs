namespace MADOC.Application.Documents;

public sealed class DocumentFactory
{
    private readonly IDocumentTypeRegistry registry;

    public DocumentFactory(IDocumentTypeRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        this.registry = registry;
    }

    public object Create(DocumentTypeKey documentType)
    {
        var descriptor = registry.GetRequired(documentType);

        try
        {
            return Activator.CreateInstance(descriptor.DocumentType)
                ?? throw new InvalidOperationException($"Не удалось создать документ типа {descriptor.DocumentType.Name}.");
        }
        catch (Exception exception) when (exception is not InvalidOperationException)
        {
            throw new InvalidOperationException(
                $"Не удалось создать документ типа {descriptor.DocumentType.Name}.",
                exception);
        }
    }
}
