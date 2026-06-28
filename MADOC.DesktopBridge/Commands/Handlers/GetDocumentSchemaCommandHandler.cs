using System.Text.Json;
using MADOC.Application.Documents;
using MADOC.Application.Forms;
using MADOC.DesktopBridge.Protocol;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Commands;

public sealed class GetDocumentSchemaCommandHandler : IBridgeCommandHandler
{
    private readonly DocumentFormSchemaService schemaService;

    public GetDocumentSchemaCommandHandler(DocumentFormSchemaService schemaService)
    {
        ArgumentNullException.ThrowIfNull(schemaService);
        this.schemaService = schemaService;
    }

    public string CommandName => BridgeCommandNames.GetDocumentSchema;

    public ValueTask<object?> HandleAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        try
        {
            var request = PayloadReader.ReadRequired<DocumentTypePayload>(payload);
            request.Validate();

            var schema = schemaService.GetSchema(new DocumentTypeKey(request.DocumentType));
            return ValueTask.FromResult<object?>(DocumentFormSchemaDto.FromApplicationSchema(schema));
        }
        catch (ArgumentException exception)
        {
            throw new BridgeRequestException(BridgeErrorCode.InvalidPayload, exception.Message);
        }
        catch (InvalidOperationException exception)
        {
            throw new BridgeRequestException(BridgeErrorCode.CommandFailed, exception.Message);
        }
    }
}
