namespace MADOC.DesktopBridge.Protocol;

public sealed class BridgeResponse
{
    public string? Id { get; }

    public bool Success { get; }

    public object? Data { get; }

    public BridgeError? Error { get; }

    private BridgeResponse(string? id, bool success, object? data, BridgeError? error)
    {
        Id = id;
        Success = success;
        Data = data;
        Error = error;
    }

    public static BridgeResponse Ok(string? id, object? data)
    {
        return new BridgeResponse(id, true, data, null);
    }

    public static BridgeResponse Fail(string? id, BridgeError error)
    {
        ArgumentNullException.ThrowIfNull(error);
        return new BridgeResponse(id, false, null, error);
    }
}
