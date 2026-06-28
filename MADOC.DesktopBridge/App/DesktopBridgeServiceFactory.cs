using MADOC.Application.Documents;
using MADOC.Application.Forms;
using MADOC.Application.Lists;
using MADOC.Application.Printing;
using MADOC.Application.Validation;
using MADOC.DesktopBridge.Commands;

namespace MADOC.DesktopBridge.App;

public static class DesktopBridgeServiceFactory
{
    public static DesktopBridgeServices CreateDefault()
    {
        var registry = new DocumentTypeRegistry();
        var documentFactory = new DocumentFactory(registry);
        var valueMapper = new DocumentValueMapper();
        var formSchemaService = new DocumentFormSchemaService(registry);
        var fieldOptionsService = new DocumentFieldOptionsService(documentFactory, valueMapper);
        var validationService = new DocumentValidationService(documentFactory, valueMapper);
        var templateProvider = FileSystemPrintTemplateProvider.FromApplicationBaseDirectory();
        var printFormService = new PrintFormService(registry, documentFactory, valueMapper, templateProvider);

        return new DesktopBridgeServices(
            registry,
            documentFactory,
            valueMapper,
            formSchemaService,
            fieldOptionsService,
            validationService,
            printFormService);
    }

    public static BridgeCommandDispatcher CreateDispatcher(DesktopBridgeServices services)
    {
        ArgumentNullException.ThrowIfNull(services);

        return new BridgeCommandDispatcher(new IBridgeCommandHandler[]
        {
            new HealthCommandHandler(),
            new GetDocumentTypesCommandHandler(services.DocumentTypeRegistry),
            new GetDocumentSchemaCommandHandler(services.FormSchemaService),
            new GetFieldOptionsCommandHandler(services.FieldOptionsService),
            new ValidateDocumentCommandHandler(services.ValidationService),
            new GeneratePrintHtmlCommandHandler(services.PrintFormService),
        });
    }
}
