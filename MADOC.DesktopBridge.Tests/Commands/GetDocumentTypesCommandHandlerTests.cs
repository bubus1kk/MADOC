using System.Text.Json;
using MADOC.Application.Documents;
using MADOC.DesktopBridge.Commands;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Tests.Commands;

[TestClass]
public sealed class GetDocumentTypesCommandHandlerTests
{
    [TestMethod]
    public async Task HandleAsync_ReturnsRegisteredDocumentTypes()
    {
        using var document = JsonDocument.Parse("{}");

        var registry = new DocumentTypeRegistry();
        var handler = new GetDocumentTypesCommandHandler(registry);

        var result = await handler.HandleAsync(document.RootElement, CancellationToken.None);

        Assert.IsNotNull(result);

        var json = JsonSerializer.Serialize(result, BridgeJsonSerializerOptions.Default);
        using var resultDocument = JsonDocument.Parse(json);

        var root = resultDocument.RootElement;

        Assert.IsTrue(root.TryGetProperty("documentTypes", out var documentTypes));
        Assert.AreEqual(JsonValueKind.Array, documentTypes.ValueKind);
        Assert.IsTrue(documentTypes.GetArrayLength() > 0);

        var firstDocumentType = documentTypes[0];

        Assert.IsTrue(firstDocumentType.TryGetProperty("key", out _));
        Assert.IsTrue(firstDocumentType.TryGetProperty("displayName", out _));
        Assert.IsTrue(firstDocumentType.TryGetProperty("templateFileName", out _));
    }
}