namespace MADOC.DesktopBridge.Protocol;

public sealed class BridgeRequestException : Exception
{
    public string Code { get; }

    public object? Details { get; }

    public BridgeRequestException(string code, string message, object? details = null)
        : base(message)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Код ошибки bridge не может быть пустым.", nameof(code));
        }

        Code = code;
        Details = details;
    }
}
