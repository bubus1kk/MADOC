using System.Text.Json;
using MADOC.Application.Documents;
using MADOC.Application.Validation;
using MADOC.DesktopBridge.Protocol;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Commands;

public sealed class ValidateDocumentCommandHandler : IBridgeCommandHandler
{
    private readonly DocumentValidationService validationService;

    public ValidateDocumentCommandHandler(DocumentValidationService validationService)
    {
        ArgumentNullException.ThrowIfNull(validationService);
        this.validationService = validationService;
    }

    public string CommandName => BridgeCommandNames.ValidateDocument;

    public ValueTask<object?> HandleAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        try
        {
            var payloadModel = PayloadReader.ReadRequired<ValidateDocumentPayload>(payload);
            payloadModel.Validate();

            var request = new DocumentValidationRequest(
                new DocumentTypeKey(payloadModel.DocumentType),
                BridgeValueDictionary.From(payloadModel.Values));

            var response = validationService.Validate(request);

            return ValueTask.FromResult<object?>(new
            {
                documentType = response.DocumentType.Value,
                response.IsValid,
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
