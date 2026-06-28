using System.Text.Json;
using MADOC.DesktopBridge.Commands;
using MADOC.DesktopBridge.Protocol;

namespace MADOC.DesktopBridge.Tests.Protocol;

[TestClass]
public sealed class JsonLineBridgeServerTests
{
    [TestMethod]
    public async Task RunAsync_WhenHealthRequestReceived_WritesSuccessResponse()
    {
        var input = new StringReader("""
        {"id":"1","command":"health","payload":{}}
        """);

        var output = new StringWriter();
        var diagnostics = new StringWriter();

        var dispatcher = new BridgeCommandDispatcher(new IBridgeCommandHandler[]
        {
            new HealthCommandHandler()
        });

        var server = new JsonLineBridgeServer(input, output, diagnostics, dispatcher);

        await server.RunAsync();

        var responseText = output.ToString().Trim();

        Assert.IsFalse(string.IsNullOrWhiteSpace(responseText));

        using var responseDocument = JsonDocument.Parse(responseText);
        var root = responseDocument.RootElement;

        Assert.AreEqual("1", root.GetProperty("id").GetString());
        Assert.IsTrue(root.GetProperty("success").GetBoolean());

        var data = root.GetProperty("data");

        Assert.AreEqual("ok", data.GetProperty("status").GetString());
        Assert.AreEqual("MADOC.DesktopBridge", data.GetProperty("service").GetString());
    }

    [TestMethod]
    public async Task RunAsync_WhenUnknownCommandReceived_WritesFailedResponse()
    {
        var input = new StringReader("""
        {"id":"2","command":"unknownCommand","payload":{}}
        """);

        var output = new StringWriter();
        var diagnostics = new StringWriter();

        var dispatcher = new BridgeCommandDispatcher(new IBridgeCommandHandler[]
        {
            new HealthCommandHandler()
        });

        var server = new JsonLineBridgeServer(input, output, diagnostics, dispatcher);

        await server.RunAsync();

        var responseText = output.ToString().Trim();

        using var responseDocument = JsonDocument.Parse(responseText);
        var root = responseDocument.RootElement;

        Assert.AreEqual("2", root.GetProperty("id").GetString());
        Assert.IsFalse(root.GetProperty("success").GetBoolean());

        var error = root.GetProperty("error");

        Assert.AreEqual(BridgeErrorCode.UnknownCommand, error.GetProperty("code").GetString());
    }
}