using MADOC.Domain.Validation.ListDependencies;

namespace MADOC.Domain.Validation.Lists
{
    public interface IListConfigurationProvider : IListDependencySchemaProvider
    {
        DocumentListCatalog GetListCatalog();
    }
}
