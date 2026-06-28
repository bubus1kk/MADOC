using System.Text.Json;
using MADOC.DesktopBridge.Commands;
using MADOC.DesktopBridge.Protocol;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Tests.Serialization;

[TestClass]
public sealed class PayloadReaderTests
{
    [TestMethod]
    public void ReadRequired_WhenPayloadIsValid_ReturnsPayloadModel()
    {
        using var document = JsonDocument.Parse("""
        {
            "documentType": "certificate_request"
        }
        """);

        var payload = PayloadReader.ReadRequired<DocumentTypePayload>(document.RootElement);

        Assert.AreEqual("certificate_request", payload.DocumentType);
    }

    [TestMethod]
    public void ReadRequired_WhenPayloadIsUndefined_ThrowsBridgeRequestException()
    {
        var exception = Assert.ThrowsExactly<BridgeRequestException>(() =>
        {
            _ = PayloadReader.ReadRequired<DocumentTypePayload>(default);
        });

        Assert.AreEqual(BridgeErrorCode.InvalidPayload, exception.Code);
    }

    [TestMethod]
    public void ReadRequired_WhenPayloadHasWrongShape_ThrowsBridgeRequestException()
    {
        using var document = JsonDocument.Parse("""
        "not-an-object"
        """);

        var exception = Assert.ThrowsExactly<BridgeRequestException>(() =>
        {
            _ = PayloadReader.ReadRequired<DocumentTypePayload>(document.RootElement);
        });

        Assert.AreEqual(BridgeErrorCode.InvalidPayload, exception.Code);
    }
}