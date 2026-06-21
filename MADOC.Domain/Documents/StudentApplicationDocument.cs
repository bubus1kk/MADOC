using MADOC.Domain.Validation.ListDependencies;
using MADOC.Domain.Validation.Lists;

namespace MADOC.Domain.Documents
{
    public class StudentApplicationDocument : BaseDocument, IListConfigurationProvider
    {
        public DocumentListCatalog GetListCatalog()
        {
            throw new NotImplementedException();
        }

        public ListDependencySchema GetListDependencySchema()
        {
            throw new NotImplementedException();
        }
    }
}
