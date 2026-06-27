using MADOC.Application.Documents;
using MADOC.Application.Forms;
using MADOC.Application.Lists;
using MADOC.Application.Printing;
using MADOC.Application.Validation;

namespace MADOC.Application.Tests.TestSupport;

internal static class ApplicationTestServices
{
    public static IDocumentTypeRegistry CreateDefaultRegistry()
    {
        return new DocumentTypeRegistry();
    }

    public static DocumentFactory CreateDocumentFactory(IDocumentTypeRegistry? registry = null)
    {
        return new DocumentFactory(registry ?? CreateDefaultRegistry());
    }

    public static DocumentValueMapper CreateValueMapper()
    {
        return new DocumentValueMapper();
    }

    public static DocumentFormSchemaService CreateFormSchemaService(IDocumentTypeRegistry? registry = null)
    {
        return new DocumentFormSchemaService(registry ?? CreateDefaultRegistry());
    }

    public static DocumentFieldOptionsService CreateFieldOptionsService(IDocumentTypeRegistry? registry = null)
    {
        return new DocumentFieldOptionsService(CreateDocumentFactory(registry), CreateValueMapper());
    }

    public static DocumentValidationService CreateValidationService(IDocumentTypeRegistry? registry = null)
    {
        return new DocumentValidationService(CreateDocumentFactory(registry), CreateValueMapper());
    }

    public static PrintFormService CreatePrintFormService(IDocumentTypeRegistry registry, IPrintTemplateProvider templateProvider)
    {
        return new PrintFormService(registry, CreateDocumentFactory(registry), CreateValueMapper(), templateProvider);
    }
}
