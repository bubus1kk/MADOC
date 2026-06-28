using System.Text.Json;
using MADOC.Application.Documents;
using MADOC.Application.Printing;
using MADOC.DesktopBridge.Protocol;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Commands;

public sealed class GeneratePrintHtmlCommandHandler : IBridgeCommandHandler
{
    private readonly PrintFormService printFormService;

    public GeneratePrintHtmlCommandHandler(PrintFormService printFormService)
    {
        ArgumentNullException.ThrowIfNull(printFormService);
        this.printFormService = printFormService;
    }

    public string CommandName => BridgeCommandNames.GeneratePrintHtml;

    public ValueTask<object?> HandleAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        try
        {
            var payloadModel = PayloadReader.ReadRequired<GeneratePrintHtmlPayload>(payload);
            payloadModel.Validate();

            var request = new PrintFormRequest(
                new DocumentTypeKey(payloadModel.DocumentType),
                BridgeValueDictionary.From(payloadModel.Values),
                BridgeValueDictionary.From(payloadModel.GeneratedFields),
                payloadModel.TemplateFileName,
                payloadModel.StrictMode,
                payloadModel.HtmlEncodeValues);

            var response = printFormService.Generate(request);

            return ValueTask.FromResult<object?>(new
            {
                documentType = response.DocumentType.Value,
                response.IsSuccess,
                response.HtmlContent,
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
