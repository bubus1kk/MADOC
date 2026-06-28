using System.Text.Json;
using MADOC.DesktopBridge.Commands;
using MADOC.DesktopBridge.Protocol;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Tests.Commands;

[TestClass]
public sealed class BridgeCommandDispatcherTests
{
    [TestMethod]
    public async Task DispatchAsync_WhenCommandExists_CallsHandler()
    {
        using var document = JsonDocument.Parse("{}");

        var dispatcher = new BridgeCommandDispatcher(new IBridgeCommandHandler[]
        {
            new HealthCommandHandler()
        });

        var result = await dispatcher.DispatchAsync(
            BridgeCommandNames.Health,
            document.RootElement,
            CancellationToken.None);

        Assert.IsNotNull(result);

        var json = JsonSerializer.Serialize(result, BridgeJsonSerializerOptions.Default);
        using var resultDocument = JsonDocument.Parse(json);

        Assert.AreEqual("ok", resultDocument.RootElement.GetProperty("status").GetString());
    }

    [TestMethod]
    public async Task DispatchAsync_WhenCommandDoesNotExist_ThrowsBridgeRequestException()
    {
        using var document = JsonDocument.Parse("{}");

        var dispatcher = new BridgeCommandDispatcher(new IBridgeCommandHandler[]
        {
            new HealthCommandHandler()
        });

        var exception = await Assert.ThrowsExactlyAsync<BridgeRequestException>(async () =>
        {
            await dispatcher.DispatchAsync(
                "unknownCommand",
                document.RootElement,
                CancellationToken.None);
        });

        Assert.AreEqual(BridgeErrorCode.UnknownCommand, exception.Code);
    }

    [TestMethod]
    public void Constructor_WhenHandlersListIsEmpty_ThrowsArgumentException()
    {
        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = new BridgeCommandDispatcher(Array.Empty<IBridgeCommandHandler>());
        });
    }
}