using MADOC.Domain.Core.Validation.ListDependencies;

namespace MADOC.Domain.Core.Validation.Lists
{
    public interface IListConfigurationProvider : IListDependencySchemaProvider
    {
        DocumentListCatalog GetListCatalog();
    }
}
