using System.Text.Json;
using MADOC.Application.Documents;
using MADOC.Application.Lists;
using MADOC.DesktopBridge.Protocol;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Commands;

public sealed class GetFieldOptionsCommandHandler : IBridgeCommandHandler
{
    private readonly DocumentFieldOptionsService optionsService;

    public GetFieldOptionsCommandHandler(DocumentFieldOptionsService optionsService)
    {
        ArgumentNullException.ThrowIfNull(optionsService);
        this.optionsService = optionsService;
    }

    public string CommandName => BridgeCommandNames.GetFieldOptions;

    public ValueTask<object?> HandleAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        try
        {
            var payloadModel = PayloadReader.ReadRequired<GetFieldOptionsPayload>(payload);
            payloadModel.Validate();

            var request = new DocumentFieldOptionsRequest(
                new DocumentTypeKey(payloadModel.DocumentType),
                payloadModel.FieldName,
                BridgeValueDictionary.From(payloadModel.Values));

            var response = optionsService.GetOptions(request);

            return ValueTask.FromResult<object?>(new
            {
                response.IsSuccess,
                response.Options,
                response.Errors
            });
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
