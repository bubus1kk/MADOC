using System.Text.Json;
using MADOC.DesktopBridge.Serialization;

namespace MADOC.DesktopBridge.Tests.Serialization;

[TestClass]
public sealed class BridgeValueDictionaryTests
{
    [TestMethod]
    public void From_WhenValuesAreNull_ReturnsEmptyDictionary()
    {
        var result = BridgeValueDictionary.From(null);

        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void From_ConvertsJsonElementsToClrValues()
    {
        var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>("""
        {
            "name": "Иван",
            "age": 18,
            "price": 12.5,
            "isActive": true,
            "comment": null
        }
        """);

        var result = BridgeValueDictionary.From(values);

        Assert.AreEqual("Иван", result["name"]);
        Assert.AreEqual(18, result["age"]);
        Assert.AreEqual(12.5m, result["price"]);
        Assert.AreEqual(true, result["isActive"]);
        Assert.IsNull(result["comment"]);
    }
}