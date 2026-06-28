using System.Text.Json;
using MADOC.DesktopBridge.Commands;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Protocol;

public sealed class JsonLineBridgeServer
{
    private readonly TextReader input;

    private readonly TextWriter output;

    private readonly TextWriter diagnostics;

    private readonly BridgeCommandDispatcher dispatcher;

    public JsonLineBridgeServer(
        TextReader input,
        TextWriter output,
        TextWriter diagnostics,
        BridgeCommandDispatcher dispatcher)
    {
        ArgumentNullException.ThrowIfNull(input);
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(diagnostics);
        ArgumentNullException.ThrowIfNull(dispatcher);

        this.input = input;
        this.output = output;
        this.diagnostics = diagnostics;
        this.dispatcher = dispatcher;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var line = await input.ReadLineAsync(cancellationToken);

            if (line is null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            var response = await HandleLineAsync(line, cancellationToken);
            var responseJson = JsonSerializer.Serialize(response, BridgeJsonSerializerOptions.Default);

            await output.WriteLineAsync(responseJson.AsMemory(), cancellationToken);
            await output.FlushAsync(cancellationToken);
        }
    }

    private async Task<BridgeResponse> HandleLineAsync(string line, CancellationToken cancellationToken)
    {
        string? requestId = null;

        try
        {
            var request = JsonSerializer.Deserialize<BridgeRequest>(line, BridgeJsonSerializerOptions.Default);

            if (request is null)
            {
                return BridgeResponse.Fail(
                    null,
                    new BridgeError(BridgeErrorCode.InvalidRequest, "Пустой bridge-запрос."));
            }

            requestId = request.Id;
            request.Validate();

            var result = await dispatcher.DispatchAsync(request.Command, request.Payload, cancellationToken);
            return BridgeResponse.Ok(request.Id, result);
        }
        catch (BridgeRequestException exception)
        {
            return BridgeResponse.Fail(
                requestId,
                new BridgeError(exception.Code, exception.Message, exception.Details));
        }
        catch (JsonException exception)
        {
            return BridgeResponse.Fail(
                requestId,
                new BridgeError(BridgeErrorCode.InvalidRequest, "Bridge-запрос должен быть корректной JSON-строкой.", exception.Message));
        }
        catch (Exception exception)
        {
            await diagnostics.WriteLineAsync(exception.ToString());

            return BridgeResponse.Fail(
                requestId,
                new BridgeError(BridgeErrorCode.InternalError, "Внутренняя ошибка DesktopBridge.", exception.Message));
        }
    }
}
