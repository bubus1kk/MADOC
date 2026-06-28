using System.Text.Json;

namespace MADOC.DesktopBridge.Protocol;

public sealed class BridgeRequest
{
    public string? Id { get; init; }

    public string Command { get; init; } = string.Empty;

    public JsonElement Payload { get; init; }

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(Command))
        {
            throw new BridgeRequestException(
                BridgeErrorCode.InvalidRequest,
                "Поле command обязательно для bridge-запроса.");
        }
    }
}
