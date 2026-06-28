namespace MADOC.DesktopBridge.Protocol;

public sealed class BridgeError
{
    public string Code { get; }

    public string Message { get; }

    public object? Details { get; }

    public BridgeError(string code, string message, object? details = null)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            throw new ArgumentException("Код ошибки bridge не может быть пустым.", nameof(code));
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Сообщение ошибки bridge не может быть пустым.", nameof(message));
        }

        Code = code;
        Message = message;
        Details = details;
    }
}
