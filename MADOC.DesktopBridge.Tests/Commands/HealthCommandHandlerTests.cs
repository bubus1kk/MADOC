using System.Text.Json;
using MADOC.DesktopBridge.Commands;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Tests.Commands;

[TestClass]
public sealed class HealthCommandHandlerTests
{
    [TestMethod]
    public async Task HandleAsync_ReturnsOkStatus()
    {
        using var document = JsonDocument.Parse("{}");
        var handler = new HealthCommandHandler();

        var result = await handler.HandleAsync(document.RootElement, CancellationToken.None);

        Assert.IsNotNull(result);

        var json = JsonSerializer.Serialize(result, BridgeJsonSerializerOptions.Default);
        using var resultDocument = JsonDocument.Parse(json);

        var root = resultDocument.RootElement;

        Assert.AreEqual("ok", root.GetProperty("status").GetString());
        Assert.AreEqual("MADOC.DesktopBridge", root.GetProperty("service").GetString());
        Assert.AreEqual("json-lines", root.GetProperty("protocol").GetString());
    }
}