using System.Text.Json;

namespace MADOC.DesktopBridge.Commands;

public sealed class HealthCommandHandler : IBridgeCommandHandler
{
    public string CommandName => BridgeCommandNames.Health;

    public ValueTask<object?> HandleAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var result = new
        {
            status = "ok",
            service = "MADOC.DesktopBridge",
            protocol = "json-lines",
            utcTime = DateTimeOffset.UtcNow
        };

        return ValueTask.FromResult<object?>(result);
    }
}
