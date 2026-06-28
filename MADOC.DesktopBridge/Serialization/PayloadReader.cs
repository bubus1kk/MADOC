using System.Text.Json;
using MADOC.DesktopBridge.Protocol;

namespace MADOC.DesktopBridge.Serialization;

public static class PayloadReader
{
    public static T ReadRequired<T>(JsonElement payload)
    {
        try
        {
            if (payload.ValueKind is JsonValueKind.Undefined or JsonValueKind.Null)
            {
                throw new BridgeRequestException(BridgeErrorCode.InvalidPayload, "Payload обязателен для этой команды");
            }

            var result = payload.Deserialize<T>(BridgeJsonSerializerOptions.Default);

            if (result is null)
            {
                throw new BridgeRequestException(BridgeErrorCode.InvalidPayload, "Payload не удалось прочитать");
            }

            return result;
        }
        catch (BridgeRequestException)
        {
            throw;
        }
        catch (JsonException exception)
        {
            throw new BridgeRequestException(BridgeErrorCode.InvalidPayload, "Payload команды имеет некорректный формат",
                exception.Message);
        }
    }
}
