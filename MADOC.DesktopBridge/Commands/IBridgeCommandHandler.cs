using System.Text.Json;

namespace MADOC.DesktopBridge.Commands;

public interface IBridgeCommandHandler
{
    string CommandName { get; }

    ValueTask<object?> HandleAsync(JsonElement payload, CancellationToken cancellationToken);
}
