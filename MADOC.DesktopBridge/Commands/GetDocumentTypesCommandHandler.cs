using System.Text.Json;
using MADOC.Application.Documents;

namespace MADOC.DesktopBridge.Commands;

public sealed class GetDocumentTypesCommandHandler : IBridgeCommandHandler
{
    private readonly IDocumentTypeRegistry registry;

    public GetDocumentTypesCommandHandler(IDocumentTypeRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);
        this.registry = registry;
    }

    public string CommandName => BridgeCommandNames.GetDocumentTypes;

    public ValueTask<object?> HandleAsync(JsonElement payload, CancellationToken cancellationToken)
    {
        var documentTypes = registry
            .GetAll()
            .Select(descriptor => new DocumentTypeDto(
                descriptor.Key.Value,
                descriptor.DisplayName,
                descriptor.TemplateFileName))
            .ToArray();

        return ValueTask.FromResult<object?>(new { documentTypes });
    }
}
