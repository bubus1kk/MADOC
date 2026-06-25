namespace MADOC.Application.Documents;

public interface IDocumentTypeRegistry
{
    IReadOnlyList<DocumentTypeDescriptor> GetAll();

    bool TryGet(DocumentTypeKey key, out DocumentTypeDescriptor? descriptor);

    DocumentTypeDescriptor GetRequired(DocumentTypeKey key);
}
