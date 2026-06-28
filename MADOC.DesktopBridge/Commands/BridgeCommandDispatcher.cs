using System.Text.Json;
using MADOC.DesktopBridge.Protocol;

namespace MADOC.DesktopBridge.Commands;

public sealed class BridgeCommandDispatcher
{
    private readonly IReadOnlyDictionary<string, IBridgeCommandHandler> handlers;

    public BridgeCommandDispatcher(IEnumerable<IBridgeCommandHandler> handlers)
    {
        ArgumentNullException.ThrowIfNull(handlers);

        var handlerList = handlers.ToArray();

        if (handlerList.Length == 0)
        {
            throw new ArgumentException("Список обработчиков bridge-команд не может быть пустым.", nameof(handlers));
        }

        var dictionary = new Dictionary<string, IBridgeCommandHandler>(StringComparer.OrdinalIgnoreCase);

        foreach (var handler in handlerList)
        {
            if (string.IsNullOrWhiteSpace(handler.CommandName))
            {
                throw new InvalidOperationException($"Обработчик {handler.GetType().Name} имеет пустое имя команды.");
            }

            if (!dictionary.TryAdd(handler.CommandName, handler))
            {
                throw new InvalidOperationException($"Bridge-команда \"{handler.CommandName}\" зарегистрирована несколько раз.");
            }
        }

        this.handlers = dictionary;
    }

    public ValueTask<object?> DispatchAsync(
        string commandName,
        JsonElement payload,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(commandName))
        {
            throw new BridgeRequestException(
                BridgeErrorCode.InvalidRequest,
                "Имя bridge-команды не может быть пустым.");
        }

        if (!handlers.TryGetValue(commandName, out var handler))
        {
            throw new BridgeRequestException(
                BridgeErrorCode.UnknownCommand,
                $"Bridge-команда \"{commandName}\" не зарегистрирована.");
        }

        return handler.HandleAsync(payload, cancellationToken);
    }
}
