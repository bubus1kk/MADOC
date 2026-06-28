using MADOC.Application.Documents;
using MADOC.Application.Forms;
using MADOC.Application.Lists;
using MADOC.Application.Printing;
using MADOC.Application.Validation;

namespace MADOC.DesktopBridge.App;

public sealed class DesktopBridgeServices
{
    public IDocumentTypeRegistry DocumentTypeRegistry { get; }

    public DocumentFactory DocumentFactory { get; }

    public DocumentValueMapper DocumentValueMapper { get; }

    public DocumentFormSchemaService FormSchemaService { get; }

    public DocumentFieldOptionsService FieldOptionsService { get; }

    public DocumentValidationService ValidationService { get; }

    public PrintFormService PrintFormService { get; }


    public DesktopBridgeServices(
        IDocumentTypeRegistry documentTypeRegistry,
        DocumentFactory documentFactory,
        DocumentValueMapper documentValueMapper,
        DocumentFormSchemaService formSchemaService,
        DocumentFieldOptionsService fieldOptionsService,
        DocumentValidationService validationService,
        PrintFormService printFormService)
    {
        ArgumentNullException.ThrowIfNull(documentTypeRegistry);
        ArgumentNullException.ThrowIfNull(documentFactory);
        ArgumentNullException.ThrowIfNull(documentValueMapper);
        ArgumentNullException.ThrowIfNull(formSchemaService);
        ArgumentNullException.ThrowIfNull(fieldOptionsService);
        ArgumentNullException.ThrowIfNull(validationService);
        ArgumentNullException.ThrowIfNull(printFormService);

        DocumentTypeRegistry = documentTypeRegistry;
        DocumentFactory = documentFactory;
        DocumentValueMapper = documentValueMapper;
        FormSchemaService = formSchemaService;
        FieldOptionsService = fieldOptionsService;
        ValidationService = validationService;
        PrintFormService = printFormService;
    }
}
