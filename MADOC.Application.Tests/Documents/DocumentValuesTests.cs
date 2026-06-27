using MADOC.Application.Documents;

namespace MADOC.Application.Tests.Documents;

[TestClass]
public sealed class DocumentValuesTests
{
    [TestMethod]
    public void Constructor_CopiesInputDictionary()
    {
        var source = new Dictionary<string, object?>
        {
            ["Name"] = "Иванов Иван"
        };

        var values = new DocumentValues(source);
        source["Name"] = "Петров Петр";

        Assert.AreEqual("Иванов Иван", values.Values["Name"]);
    }

    [TestMethod]
    public void Empty_ReturnsEmptyContainer()
    {
        var values = DocumentValues.Empty;

        Assert.AreEqual(0, values.Values.Count);
    }

    [TestMethod]
    public void Constructor_ThrowsForNullDictionary()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new DocumentValues(null!));
    }
}
